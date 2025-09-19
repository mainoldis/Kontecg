using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Events.Bus.Factories;
using Kontecg.Events.Bus.Handlers;
using Kontecg.MassTransit.Abstractions;
using Kontecg.Threading;
using MassTransit;

namespace Kontecg.Events.Bus
{
    public class DistributedEventBus : IEventBus, IDistributedEventBus
    {
        private readonly IEventBus _innerEventBus;
        private readonly IBusControl _massTransitBus;
        private readonly IEventPublishingStrategy _publishingStrategy;
        private readonly IEventMessageMapper _messageMapper;

        public DistributedEventBus(
            IEventBus innerEventBus,
            IBusControl massTransitBus,
            IEventPublishingStrategy publishingStrategy,
            IEventMessageMapper messageMapper)
        {
            _innerEventBus = innerEventBus ?? throw new ArgumentNullException(nameof(innerEventBus));
            _massTransitBus = massTransitBus ?? throw new ArgumentNullException(nameof(massTransitBus));
            _publishingStrategy = publishingStrategy ?? throw new ArgumentNullException(nameof(publishingStrategy));
            _messageMapper = messageMapper ?? throw new ArgumentNullException(nameof(messageMapper));

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IBusControl BusControl => _massTransitBus;

        #region Registration Methods (Delegated to inner EventBus)

        /// <inheritdoc />
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return _innerEventBus.Register(action);
        }

        /// <inheritdoc />
        public IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            return _innerEventBus.AsyncRegister(action);
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return _innerEventBus.Register(handler);
        }

        /// <inheritdoc />
        public IDisposable AsyncRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return _innerEventBus.AsyncRegister(handler);
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData, THandler>() where TEventData : IEventData where THandler : IEventHandler, new()
        {
            return _innerEventBus.Register<TEventData, THandler>();
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return _innerEventBus.Register(eventType, handler);
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            return _innerEventBus.Register<TEventData>(factory);
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandlerFactory factory)
        {
            return _innerEventBus.Register(eventType, factory);
        }

        #endregion

        #region Unregistration Methods (Delegated to inner EventBus)

        /// <inheritdoc />
        public void UnRegister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            _innerEventBus.UnRegister(action);
        }

        /// <inheritdoc />
        public void AsyncUnRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            _innerEventBus.AsyncUnRegister(action);
        }

        /// <inheritdoc />
        public void UnRegister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            _innerEventBus.UnRegister(handler);
        }

        /// <inheritdoc />
        public void AsyncUnRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            _innerEventBus.AsyncUnRegister(handler);
        }

        /// <inheritdoc />
        public void UnRegister(Type eventType, IEventHandler handler)
        {
            _innerEventBus.UnRegister(eventType, handler);
        }

        /// <inheritdoc />
        public void UnRegister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            _innerEventBus.UnRegister<TEventData>(factory);
        }

        /// <inheritdoc />
        public void UnRegister(Type eventType, IEventHandlerFactory factory)
        {
            _innerEventBus.UnRegister(eventType, factory);
        }

        /// <inheritdoc />
        public void UnRegisterAll<TEventData>() where TEventData : IEventData
        {
            _innerEventBus.UnRegisterAll<TEventData>();
        }

        /// <inheritdoc />
        public void UnRegisterAll(Type eventType)
        {
            _innerEventBus.UnRegisterAll(eventType);
        }

        #endregion

        #region Trigger Methods (With added MassTransit integration)

        /// <inheritdoc />
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            // 1. First execute original local EventBus
            _innerEventBus.Trigger(eventData);
            // 2. Then evaluate and publish via MassTransit if applicable
            TryPublishToMassTransit(eventData);
        }

        /// <inheritdoc />
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            _innerEventBus.Trigger(eventSource, eventData);
            TryPublishToMassTransit(eventData);
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, IEventData eventData)
        {
            _innerEventBus.Trigger(eventType, eventData);
            TryPublishToMassTransit(eventData);
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            _innerEventBus.Trigger(eventType, eventSource, eventData);
            TryPublishToMassTransit(eventData);
        }

        /// <inheritdoc />
        public async Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            await _innerEventBus.TriggerAsync(eventData);
            await TryPublishToMassTransitAsync(eventData);
        }

        /// <inheritdoc />
        public async Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            await _innerEventBus.TriggerAsync(eventSource, eventData);
            await TryPublishToMassTransitAsync(eventData);
        }

        /// <inheritdoc />
        public async Task TriggerAsync(Type eventType, IEventData eventData)
        {
            await _innerEventBus.TriggerAsync(eventType, eventData);
            await TryPublishToMassTransitAsync(eventData);
        }

        /// <inheritdoc />
        public async Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            await _innerEventBus.TriggerAsync(eventType, eventSource, eventData);
            await TryPublishToMassTransitAsync(eventData);
        }

        #endregion

        #region Private methods for MassTransit publishing

        /// <summary>
        /// Attempts to publish an event via MassTransit synchronously.
        /// </summary>
        /// <param name="eventData">Event to publish</param>
        /// <remarks>
        /// <para>
        /// This method handles strategy evaluation, mapping and synchronous publishing.
        /// MassTransit errors do not affect local publishing already completed.
        /// </para>
        /// </remarks>
        private void TryPublishToMassTransit<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            if (eventData == null) return;

            try
            {
                // Check if should be published via MassTransit
                if (!_publishingStrategy.ShouldPublishToMassTransit(eventData))
                {
                    Logger.Debug($"Event {typeof(TEventData).Name} will not be published via MassTransit according to configured strategy");
                    return;
                }

                Logger.Debug($"Publishing event {typeof(TEventData).Name} via MassTransit (synchronous)");

                // Map event to MassTransit message
                object message = _messageMapper.MapToMessage(eventData);

                // Publish asynchronously
                AsyncHelper.RunSync(() => _massTransitBus.Publish(message));

                Logger.Info($"Event {typeof(TEventData).Name} published successfully via MassTransit (synchronous)");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error publishing event {typeof(TEventData).Name} via MassTransit (synchronous): {ex.Message}", ex);

                // Important: Don't re-throw exception so it doesn't affect local flow
                // The event was already processed locally successfully
            }
        }

        /// <summary>
        /// Attempts to publish an event via MassTransit asynchronously.
        /// </summary>
        /// <param name="eventData">Event to publish</param>
        /// <returns>Task representing the asynchronous operation</returns>
        /// <remarks>
        /// <para>
        /// This method handles strategy evaluation, mapping and asynchronous publishing.
        /// MassTransit errors do not affect local publishing already completed.
        /// </para>
        /// </remarks>
        private async Task TryPublishToMassTransitAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            if (eventData == null) return;

            try
            {
                // Check if should be published via MassTransit
                if (!_publishingStrategy.ShouldPublishToMassTransit(eventData))
                {
                    Logger.Debug($"Event {typeof(TEventData).Name} will not be published via MassTransit according to configured strategy");
                    return;
                }

                Logger.Debug($"Publishing event {typeof(TEventData).Name} via MassTransit (asynchronous)");

                // Map event to MassTransit message
                object message = _messageMapper.MapToMessage(eventData);

                // Publish asynchronously
                await _massTransitBus.Publish(message);

                Logger.Info($"Event {typeof(TEventData).Name} published successfully via MassTransit (asynchronous)");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error publishing event {typeof(TEventData).Name} via MassTransit (asynchronous): {ex.Message}", ex);

                // Important: Don't re-throw exception so it doesn't affect local flow
                // The event was already processed locally successfully
            }
        }

        #endregion
    }
}
