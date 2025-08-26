using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Events.Bus.Factories;
using Kontecg.Events.Bus.Factories.Internals;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Events.Bus.Handlers.Internals;
using Kontecg.Extensions;
using Kontecg.Threading;
using Kontecg.Threading.Extensions;

namespace Kontecg.Events.Bus
{
    /// <summary>
    /// Implements the event bus system using the Singleton pattern for global event management.
    /// </summary>
    /// <remarks>
    /// <para>
    /// EventBus is the core implementation of the event-driven architecture in the Kontecg framework.
    /// It provides a centralized mechanism for publishing and subscribing to events across the application,
    /// enabling loose coupling between components through the publish-subscribe pattern.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Thread-Safe Operations:</strong> Uses ConcurrentDictionary for thread-safe handler management</description></item>
    /// <item><description><strong>Singleton Pattern:</strong> Provides global access through EventBus.Default</description></item>
    /// <item><description><strong>Multiple Handler Types:</strong> Supports actions, handlers, factories, and type-based registration</description></item>
    /// <item><description><strong>Async Support:</strong> Full support for asynchronous event handling</description></item>
    /// <item><description><strong>Exception Handling:</strong> Robust exception handling and logging</description></item>
    /// <item><description><strong>Performance Optimized:</strong> Efficient event routing and handler invocation</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Architecture:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Handler Storage:</strong> Uses ConcurrentDictionary to store handler factories by event type</description></item>
    /// <item><description><strong>Factory Pattern:</strong> Uses IEventHandlerFactory for flexible handler lifecycle management</description></item>
    /// <item><description><strong>Event Routing:</strong> Efficiently routes events to appropriate handlers</description></item>
    /// <item><description><strong>Context Management:</strong> Handles synchronization context for async operations</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Global Access:</strong> Use EventBus.Default for application-wide event bus</description></item>
    /// <item><description><strong>Instance Creation:</strong> Create new instances for isolated event domains</description></item>
    /// <item><description><strong>Dependency Injection:</strong> Register as singleton in DI container</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> All public methods are thread-safe and can be called from multiple
    /// threads concurrently. The internal state is protected using thread-safe collections and locking mechanisms.
    /// </para>
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Event registration and unregistration are O(1) operations</description></item>
    /// <item><description>Event triggering is O(n) where n is the number of registered handlers</description></item>
    /// <item><description>Memory usage scales with the number of registered event types and handlers</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class EventBus : IEventBus
    {
        /// <summary>
        /// Thread-safe collection of all registered handler factories, organized by event type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This dictionary stores handler factories for each event type. The key is the event type,
        /// and the value is a list of handler factories that can create handlers for that event type.
        /// </para>
        /// <para>
        /// <strong>Thread Safety:</strong> Uses ConcurrentDictionary to ensure thread-safe access
        /// when multiple threads register or unregister handlers concurrently.
        /// </para>
        /// <para>
        /// <strong>Factory Pattern:</strong> Each entry contains factories rather than direct handlers,
        /// allowing for flexible handler lifecycle management and dependency injection.
        /// </para>
        /// </remarks>
        private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Creates a new EventBus instance with an empty handler factory collection and a null logger.
        /// This constructor is typically used when you need an isolated event bus instance rather than
        /// using the global EventBus.Default.
        /// </para>
        /// <para>
        /// <strong>Initialization:</strong>
        /// <list type="bullet">
        /// <item><description>Creates a new ConcurrentDictionary for thread-safe handler storage</description></item>
        /// <item><description>Sets Logger to NullLogger.Instance by default</description></item>
        /// <item><description>Prepares the event bus for handler registration</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Consider using EventBus.Default for most scenarios unless you need
        /// isolated event domains or custom configuration.
        /// </para>
        /// </remarks>
        public EventBus()
        {
            _handlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets the default global EventBus instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property provides access to a singleton EventBus instance that can be used
        /// throughout the application for global event management. It's the recommended
        /// way to access the event bus for most scenarios.
        /// </para>
        /// <para>
        /// <strong>Singleton Pattern:</strong> The default instance is created once and reused
        /// throughout the application lifecycle, ensuring consistent event handling across
        /// all components.
        /// </para>
        /// <para>
        /// <strong>Global Access:</strong> This instance is shared across the entire application,
        /// so events registered on this bus will be available to all components that use it.
        /// </para>
        /// <para>
        /// <strong>Usage Example:</strong>
        /// <code>
        /// // Register a handler
        /// EventBus.Default.Register&lt;UserCreatedEventData&gt;(eventData =>
        /// {
        ///     Console.WriteLine($"User {eventData.UserName} was created");
        /// });
        /// 
        /// // Trigger an event
        /// EventBus.Default.Trigger(new UserCreatedEventData { UserName = "John" });
        /// </code>
        /// </para>
        /// </remarks>
        public static EventBus Default { get; } = new();

        /// <summary>
        /// Gets or sets the logger used by the event bus for logging operations and errors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property allows you to configure logging for the event bus. By default, it's set
        /// to NullLogger.Instance, which means no logging occurs. You can set this to any
        /// ILogger implementation to enable logging.
        /// </para>
        /// <para>
        /// <strong>Logging Events:</strong> The event bus logs various events including:
        /// <list type="bullet">
        /// <item><description>Handler registration and unregistration</description></item>
        /// <item><description>Event triggering and processing</description></item>
        /// <item><description>Exceptions during event handling</description></item>
        /// <item><description>Performance metrics and diagnostics</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Configuration:</strong> Set this property early in the application lifecycle
        /// to ensure all event bus operations are properly logged.
        /// </para>
        /// </remarks>
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new ActionEventHandler<TEventData>(action));
        }

        /// <inheritdoc />
        public IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new AsyncActionEventHandler<TEventData>(action));
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        /// <inheritdoc />
        public IDisposable AsyncRegister<TEventData>(IAsyncEventHandler<TEventData> handler)
            where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return Register(typeof(TEventData), new TransientEventHandlerFactory<THandler>());
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            return Register(typeof(TEventData), factory);
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories => factories.Add(factory));

            return new FactoryUnregistrar(this, eventType, factory);
        }

        /// <inheritdoc />
        public void UnRegister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            SingleInstanceHandlerFactory
                                singleInstanceFactory = factory as SingleInstanceHandlerFactory;

                            if (!(singleInstanceFactory
                                    ?.HandlerInstance is ActionEventHandler<TEventData> actionHandler))
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc />
        public void AsyncUnRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            SingleInstanceHandlerFactory
                                singleInstanceFactory = factory as SingleInstanceHandlerFactory;

                            if (!(singleInstanceFactory?.HandlerInstance is AsyncActionEventHandler<TEventData>
                                    actionHandler))
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc />
        public void UnRegister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            UnRegister(typeof(TEventData), handler);
        }

        /// <inheritdoc />
        public void AsyncUnRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            UnRegister(typeof(TEventData), handler);
        }

        /// <inheritdoc />
        public void UnRegister(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                    );
                });
        }

        /// <inheritdoc />
        public void UnRegister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            UnRegister(typeof(TEventData), factory);
        }

        /// <inheritdoc />
        public void UnRegister(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <inheritdoc />
        public void UnRegisterAll<TEventData>() where TEventData : IEventData
        {
            UnRegisterAll(typeof(TEventData));
        }

        /// <inheritdoc />
        public void UnRegisterAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        /// <inheritdoc />
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger<TEventData>(typeof(TEventData), eventData);
        }

        /// <inheritdoc />
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            Trigger(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, IEventData eventData)
        {
            Trigger(eventType, null, eventData);
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            List<Exception> exceptions = new List<Exception>();

            eventData.EventSource = eventSource;

            foreach (EventTypeWithEventHandlerFactories handlerFactories in GetHandlerFactories(eventType))
            foreach (IEventHandlerFactory handlerFactory in handlerFactories.EventHandlerFactories)
            {
                Type handlerType = handlerFactory.GetHandlerType();

                if (IsAsyncEventHandler(handlerType))
                {
                    AsyncHelper.RunSync(() =>
                        TriggerAsyncHandlingExceptionAsync(handlerFactory, handlerFactories.EventType, eventData,
                            exceptions));
                }
                else if (IsEventHandler(handlerType))
                {
                    TriggerHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                }
                else
                {
                    string message =
                        $"Event handler to register for event type {eventType.Name} does not implement IEventHandler<{eventType.Name}> or IAsyncEventHandler<{eventType.Name}> interface!";
                    exceptions.Add(new KontecgException(message));
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                Type genericArg = eventType.GetGenericArguments()[0];
                Type baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    Type baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    object[] constructorArgs =
                        ((IEventDataWithInheritableGenericArgument) eventData).GetConstructorArgs();
                    IEventData baseEventData = (IEventData) Activator.CreateInstance(baseEventType, constructorArgs);
                    baseEventData.EventTime = eventData.EventTime;
                    Trigger(baseEventType, eventData.EventSource, baseEventData);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException(
                    "More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        /// <inheritdoc />
        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync<TEventData>(typeof(TEventData), eventData);
        }

        /// <inheritdoc />
        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc />
        public Task TriggerAsync(Type eventType, IEventData eventData)
        {
            return TriggerAsync(eventType, null, eventData);
        }

        /// <inheritdoc />
        public async Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            List<Exception> exceptions = new List<Exception>();

            eventData.EventSource = eventSource;

            await new SynchronizationContextRemover();

            foreach (EventTypeWithEventHandlerFactories handlerFactories in GetHandlerFactories(eventType))
            foreach (IEventHandlerFactory handlerFactory in handlerFactories.EventHandlerFactories)
            {
                Type handlerType = handlerFactory.GetHandlerType();

                if (IsAsyncEventHandler(handlerType))
                {
                    await TriggerAsyncHandlingExceptionAsync(handlerFactory, handlerFactories.EventType, eventData,
                        exceptions);
                }
                else if (IsEventHandler(handlerType))
                {
                    TriggerHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                }
                else
                {
                    string message =
                        $"Event handler to register for event type {eventType.Name} does not implement IEventHandler<{eventType.Name}> or IAsyncEventHandler<{eventType.Name}> interface!";
                    exceptions.Add(new KontecgException(message));
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                Type genericArg = eventType.GetGenericArguments()[0];
                Type baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    Type baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    object[] constructorArgs =
                        ((IEventDataWithInheritableGenericArgument) eventData).GetConstructorArgs();
                    IEventData baseEventData = (IEventData) Activator.CreateInstance(baseEventType, constructorArgs);
                    baseEventData.EventTime = eventData.EventTime;
                    await TriggerAsync(baseEventType, eventData.EventSource, baseEventData);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException(
                    "More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        private void TriggerHandlingException(IEventHandlerFactory handlerFactory, Type eventType, IEventData eventData,
            List<Exception> exceptions)
        {
            IEventHandler eventHandler = handlerFactory.GetHandler();
            try
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException(
                        $"Registered event handler for event type {eventType.Name} is null!");
                }

                Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

                MethodInfo method = handlerType.GetMethod(
                    "HandleEvent",
                    new[] {eventType}
                );

                method.Invoke(eventHandler, new object[] {eventData});
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                handlerFactory.ReleaseHandler(eventHandler);
            }
        }

        private async Task TriggerAsyncHandlingExceptionAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType,
            IEventData eventData, List<Exception> exceptions)
        {
            IEventHandler asyncEventHandler = asyncHandlerFactory.GetHandler();

            try
            {
                if (asyncEventHandler == null)
                {
                    throw new ArgumentNullException(
                        $"Registered async event handler for event type {eventType.Name} is null!");
                }

                Type asyncHandlerType = typeof(IAsyncEventHandler<>).MakeGenericType(eventType);

                MethodInfo method = asyncHandlerType.GetMethod(
                    "HandleEventAsync",
                    new[] {eventType}
                );

                await (Task) method.Invoke(asyncEventHandler, new object[] {eventData});
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                asyncHandlerFactory.ReleaseHandler(asyncEventHandler);
            }
        }

        private bool IsEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
        }

        private bool IsAsyncEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IAsyncEventHandler<>));
        }

        private IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            List<EventTypeWithEventHandlerFactories>
                handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (KeyValuePair<Type, List<IEventHandlerFactory>> handlerFactory in
                     _handlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(
                    new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type eventType, Type handlerType)
        {
            //Should trigger same type
            if (handlerType == eventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerType.IsAssignableFrom(eventType))
            {
                return true;
            }

            return false;
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return _handlerFactories.GetOrAdd(eventType, type => new List<IEventHandlerFactory>());
        }

        private class EventTypeWithEventHandlerFactories
        {
            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }

            public Type EventType { get; }

            public List<IEventHandlerFactory> EventHandlerFactories { get; }
        }

        // Reference from
        // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
        private struct SynchronizationContextRemover : INotifyCompletion
        {
            public bool IsCompleted => SynchronizationContext.Current == null;

            public void OnCompleted(Action continuation)
            {
                SynchronizationContext prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }
        }
    }
}
