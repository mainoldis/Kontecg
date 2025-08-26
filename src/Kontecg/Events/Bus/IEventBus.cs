using System;
using System.Threading.Tasks;
using Kontecg.Events.Bus.Factories;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Events.Bus
{
    /// <summary>
    /// Defines the interface for the event bus system in the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// IEventBus provides a comprehensive event-driven architecture that enables loose coupling
    /// between components through publish-subscribe patterns. The event bus allows components
    /// to communicate without direct dependencies, promoting modularity and testability.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Event Registration:</strong> Register handlers for specific event types</description></item>
    /// <item><description><strong>Event Triggering:</strong> Publish events to all registered handlers</description></item>
    /// <item><description><strong>Async Support:</strong> Both synchronous and asynchronous event handling</description></item>
    /// <item><description><strong>Handler Management:</strong> Register, unregister, and manage event handlers</description></item>
    /// <item><description><strong>Factory Support:</strong> Use factories for handler creation and lifecycle management</description></item>
    /// <item><description><strong>Type Safety:</strong> Strongly typed event data and handlers</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Event Flow:</strong>
    /// <list type="number">
    /// <item><description>Components register handlers for specific event types</description></item>
    /// <item><description>Other components trigger events with event data</description></item>
    /// <item><description>The event bus delivers events to all registered handlers</description></item>
    /// <item><description>Handlers process the events asynchronously or synchronously</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Handler Types:</strong> The event bus supports multiple handler registration patterns:
    /// <list type="bullet">
    /// <item><description><strong>Action-based:</strong> Simple lambda expressions or methods</description></item>
    /// <item><description><strong>Interface-based:</strong> Implementations of IEventHandler</description></item>
    /// <item><description><strong>Factory-based:</strong> Custom factories for handler creation</description></item>
    /// <item><description><strong>Type-based:</strong> Automatic instantiation of handler types</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IEventBus
    {
        #region Register

        /// <summary>
        /// Registers an action to handle events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the action will handle.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action to be called when events of the specified type occur.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the action.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers a simple action (delegate) to handle events. The action
        /// will be called for every occurrence of the specified event type.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This is the simplest way to register event handlers,
        /// suitable for simple scenarios where you don't need complex handler logic.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// var subscription = eventBus.Register&lt;UserCreatedEventData&gt;(eventData =>
        /// {
        ///     Console.WriteLine($"User {eventData.UserName} was created");
        /// });
        /// </code>
        /// </para>
        /// </remarks>
        IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        /// <summary>
        /// Registers an asynchronous action to handle events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the action will handle.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="action">
        /// The asynchronous action to be called when events of the specified type occur.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the action.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers an asynchronous action to handle events. The action
        /// will be called for every occurrence of the specified event type and can
        /// perform asynchronous operations.
        /// </para>
        /// <para>
        /// <strong>Async Handling:</strong> The event bus will handle the asynchronous
        /// execution of the action, allowing for non-blocking event processing.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// var subscription = eventBus.AsyncRegister&lt;UserCreatedEventData&gt;(async eventData =>
        /// {
        ///     await SendWelcomeEmailAsync(eventData.UserEmail);
        ///     await UpdateUserStatisticsAsync();
        /// });
        /// </code>
        /// </para>
        /// </remarks>
        IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData;

        /// <summary>
        /// Registers an event handler instance for events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the handler will process.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="handler">
        /// The handler instance that will process events of the specified type.
        /// The same instance will be used for all event occurrences.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the handler.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers a handler instance that implements <see cref="IEventHandler{TEventData}"/>.
        /// The same handler instance will be reused for all event occurrences, which can be
        /// useful for maintaining state or sharing resources.
        /// </para>
        /// <para>
        /// <strong>Handler Reuse:</strong> The same handler instance is used for all events,
        /// so ensure the handler is thread-safe if events can be triggered from multiple threads.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// var handler = new UserCreatedEventHandler();
        /// var subscription = eventBus.Register&lt;UserCreatedEventData&gt;(handler);
        /// </code>
        /// </para>
        /// </remarks>
        IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        /// Registers an asynchronous event handler instance for events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the handler will process.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="handler">
        /// The asynchronous handler instance that will process events of the specified type.
        /// The same instance will be used for all event occurrences.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the handler.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers an asynchronous handler instance that implements
        /// <see cref="IAsyncEventHandler{TEventData}"/>. The handler can perform
        /// asynchronous operations when processing events.
        /// </para>
        /// <para>
        /// <strong>Async Processing:</strong> The event bus will handle the asynchronous
        /// execution, allowing for non-blocking event processing.
        /// </para>
        /// </remarks>
        IDisposable AsyncRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        /// Registers an event handler type for events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the handler will process.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <typeparam name="THandler">
        /// The type of the event handler. Must implement <see cref="IEventHandler"/>
        /// and have a parameterless constructor.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the handler.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers a handler type rather than an instance. A new instance
        /// of the handler type will be created for each event occurrence, providing
        /// isolation between event processing.
        /// </para>
        /// <para>
        /// <strong>Instance Creation:</strong> The handler type must have a parameterless
        /// constructor, as the event bus will create new instances for each event.
        /// </para>
        /// <para>
        /// <strong>Isolation:</strong> Each event gets its own handler instance, which
        /// can be beneficial for avoiding state conflicts between concurrent events.
        /// </para>
        /// </remarks>
        IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData where THandler : IEventHandler, new();

        /// <summary>
        /// Registers an event handler instance for events of the specified type using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data that the handler will process.
        /// </param>
        /// <param name="handler">
        /// The handler instance that will process events of the specified type.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the handler.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method allows registration using runtime type information instead of
        /// compile-time generic types. This is useful when the event type is not known
        /// at compile time.
        /// </para>
        /// <para>
        /// <strong>Runtime Registration:</strong> This method is particularly useful
        /// for dynamic scenarios where event types are determined at runtime.
        /// </para>
        /// </remarks>
        IDisposable Register(Type eventType, IEventHandler handler);

        /// <summary>
        /// Registers an event handler factory for events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the factory will create handlers for.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="factory">
        /// The factory that will be used to create and release handlers for events
        /// of the specified type.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the factory.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method registers a factory that will be responsible for creating and
        /// releasing event handlers. This provides fine-grained control over handler
        /// lifecycle and resource management.
        /// </para>
        /// <para>
        /// <strong>Factory Control:</strong> The factory pattern allows for custom
        /// logic in handler creation, dependency injection, and resource cleanup.
        /// </para>
        /// </remarks>
        IDisposable Register<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData;

        /// <summary>
        /// Registers an event handler factory for events of the specified type using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data that the factory will create handlers for.
        /// </param>
        /// <param name="factory">
        /// The factory that will be used to create and release handlers for events
        /// of the specified type.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that can be used to unregister the factory.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method allows factory registration using runtime type information,
        /// useful for dynamic scenarios where event types are determined at runtime.
        /// </para>
        /// </remarks>
        IDisposable Register(Type eventType, IEventHandlerFactory factory);

        #endregion

        #region Unregister

        /// <summary>
        /// Unregisters an action from handling events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the action was registered for.
        /// </typeparam>
        /// <param name="action">
        /// The action to unregister. Must be the same action that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered action from the event bus.
        /// The action will no longer be called when events of the specified type occur.
        /// </para>
        /// <para>
        /// <strong>Reference Equality:</strong> The action must be the same reference
        /// that was used during registration for the unregistration to succeed.
        /// </para>
        /// </remarks>
        void UnRegister<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        /// <summary>
        /// Unregisters an asynchronous action from handling events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the action was registered for.
        /// </typeparam>
        /// <param name="action">
        /// The asynchronous action to unregister. Must be the same action that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered asynchronous action from the event bus.
        /// The action will no longer be called when events of the specified type occur.
        /// </para>
        /// </remarks>
        void AsyncUnRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData;

        /// <summary>
        /// Unregisters an event handler instance from handling events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the handler was registered for.
        /// </typeparam>
        /// <param name="handler">
        /// The handler instance to unregister. Must be the same instance that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered handler instance from the event bus.
        /// The handler will no longer process events of the specified type.
        /// </para>
        /// <para>
        /// <strong>Reference Equality:</strong> The handler must be the same reference
        /// that was used during registration for the unregistration to succeed.
        /// </para>
        /// </remarks>
        void UnRegister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        /// Unregisters an asynchronous event handler instance from handling events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the handler was registered for.
        /// </typeparam>
        /// <param name="handler">
        /// The asynchronous handler instance to unregister. Must be the same instance that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered asynchronous handler instance from the event bus.
        /// The handler will no longer process events of the specified type.
        /// </para>
        /// </remarks>
        void AsyncUnRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        /// Unregisters an event handler instance from handling events of the specified type using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data that the handler was registered for.
        /// </param>
        /// <param name="handler">
        /// The handler instance to unregister. Must be the same instance that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered handler instance using runtime type information.
        /// Useful for dynamic scenarios where event types are determined at runtime.
        /// </para>
        /// </remarks>
        void UnRegister(Type eventType, IEventHandler handler);

        /// <summary>
        /// Unregisters an event handler factory from handling events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data that the factory was registered for.
        /// </typeparam>
        /// <param name="factory">
        /// The factory to unregister. Must be the same factory that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered factory from the event bus.
        /// The factory will no longer be used to create handlers for events of the specified type.
        /// </para>
        /// </remarks>
        void UnRegister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData;

        /// <summary>
        /// Unregisters an event handler factory from handling events of the specified type using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data that the factory was registered for.
        /// </param>
        /// <param name="factory">
        /// The factory to unregister. Must be the same factory that was previously registered.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a previously registered factory using runtime type information.
        /// Useful for dynamic scenarios where event types are determined at runtime.
        /// </para>
        /// </remarks>
        void UnRegister(Type eventType, IEventHandlerFactory factory);

        /// <summary>
        /// Unregisters all event handlers for events of the specified type.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data to unregister all handlers for.
        /// </typeparam>
        /// <remarks>
        /// <para>
        /// This method removes all registered handlers (actions, handler instances, and factories)
        /// for the specified event type. This is useful for cleanup operations or when
        /// you want to completely remove all event handling for a specific event type.
        /// </para>
        /// <para>
        /// <strong>Complete Cleanup:</strong> This method will remove all types of handlers
        /// registered for the event type, including actions, handler instances, and factories.
        /// </para>
        /// </remarks>
        void UnRegisterAll<TEventData>() where TEventData : IEventData;

        /// <summary>
        /// Unregisters all event handlers for events of the specified type using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data to unregister all handlers for.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes all registered handlers for the specified event type using
        /// runtime type information. Useful for dynamic scenarios where event types are
        /// determined at runtime.
        /// </para>
        /// </remarks>
        void UnRegisterAll(Type eventType);

        #endregion

        #region Trigger

        /// <summary>
        /// Triggers an event synchronously.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data to trigger.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method triggers an event synchronously, causing all registered handlers
        /// to be executed immediately in the calling thread. The method will not return
        /// until all handlers have completed execution.
        /// </para>
        /// <para>
        /// <strong>Synchronous Execution:</strong> All handlers are executed in the
        /// calling thread, which means the method will block until all handlers complete.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// var eventData = new UserCreatedEventData { UserName = "John Doe" };
        /// eventBus.Trigger(eventData);
        /// </code>
        /// </para>
        /// </remarks>
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;

        /// <summary>
        /// Triggers an event synchronously with an event source.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data to trigger.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="eventSource">
        /// The object that is triggering the event. This will be set as the EventSource
        /// property of the event data.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method triggers an event synchronously with an event source. The event
        /// source is typically the object that is causing the event to occur and can
        /// be useful for handlers that need to know the origin of the event.
        /// </para>
        /// <para>
        /// <strong>Event Source:</strong> The event source will be set as the EventSource
        /// property of the event data before the event is triggered.
        /// </para>
        /// </remarks>
        void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        /// <summary>
        /// Triggers an event synchronously using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data to trigger.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method triggers an event synchronously using runtime type information.
        /// Useful for dynamic scenarios where the event type is determined at runtime.
        /// </para>
        /// </remarks>
        void Trigger(Type eventType, IEventData eventData);

        /// <summary>
        /// Triggers an event synchronously with an event source using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data to trigger.
        /// </param>
        /// <param name="eventSource">
        /// The object that is triggering the event.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method triggers an event synchronously with an event source using runtime
        /// type information. Useful for dynamic scenarios where the event type is determined
        /// at runtime.
        /// </para>
        /// </remarks>
        void Trigger(Type eventType, object eventSource, IEventData eventData);

        /// <summary>
        /// Triggers an event asynchronously.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data to trigger.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of triggering the event.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method triggers an event asynchronously, allowing handlers to be executed
        /// without blocking the calling thread. The method returns immediately and the
        /// event processing continues in the background.
        /// </para>
        /// <para>
        /// <strong>Asynchronous Execution:</strong> Handlers are executed asynchronously,
        /// allowing for non-blocking event processing and better responsiveness.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// var eventData = new UserCreatedEventData { UserName = "John Doe" };
        /// await eventBus.TriggerAsync(eventData);
        /// </code>
        /// </para>
        /// </remarks>
        Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData;

        /// <summary>
        /// Triggers an event asynchronously with an event source.
        /// </summary>
        /// <typeparam name="TEventData">
        /// The type of event data to trigger.
        /// Must implement <see cref="IEventData"/>.
        /// </typeparam>
        /// <param name="eventSource">
        /// The object that is triggering the event. This will be set as the EventSource
        /// property of the event data.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of triggering the event.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method triggers an event asynchronously with an event source. The event
        /// source is typically the object that is causing the event to occur.
        /// </para>
        /// <para>
        /// <strong>Asynchronous Processing:</strong> The event is processed asynchronously,
        /// allowing for non-blocking operation while maintaining the event source context.
        /// </para>
        /// </remarks>
        Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        /// <summary>
        /// Triggers an event asynchronously using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data to trigger.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of triggering the event.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method triggers an event asynchronously using runtime type information.
        /// Useful for dynamic scenarios where the event type is determined at runtime.
        /// </para>
        /// </remarks>
        Task TriggerAsync(Type eventType, IEventData eventData);

        /// <summary>
        /// Triggers an event asynchronously with an event source using runtime type information.
        /// </summary>
        /// <param name="eventType">
        /// The type of event data to trigger.
        /// </param>
        /// <param name="eventSource">
        /// The object that is triggering the event.
        /// </param>
        /// <param name="eventData">
        /// The event data to be passed to all registered handlers.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of triggering the event.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method triggers an event asynchronously with an event source using runtime
        /// type information. Useful for dynamic scenarios where the event type is determined
        /// at runtime.
        /// </para>
        /// </remarks>
        Task TriggerAsync(Type eventType, object eventSource, IEventData eventData);

        #endregion
    }
}
