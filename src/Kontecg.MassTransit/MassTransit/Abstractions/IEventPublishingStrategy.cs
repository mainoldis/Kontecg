using Kontecg.Events.Bus;
using System;

namespace Kontecg.MassTransit.Abstractions
{
    /// <summary>
    /// Defines strategies for determining whether an event should be published through MassTransit
    /// in addition to local EventBus publication.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface allows granular configuration of which events should be distributed
    /// through MassTransit vs which should remain as local events only.
    /// </para>
    /// <para>
    /// <strong>Use Cases:</strong>
    /// <list type="bullet">
    /// <item><description>Domain events that need synchronization between bounded contexts</description></item>
    /// <item><description>Integration events with external systems</description></item>
    /// <item><description>Notifications that require distributed processing</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IEventPublishingStrategy
    {
        /// <summary>
        /// Determines if a specific event should be published through MassTransit.
        /// </summary>
        /// <typeparam name="TEventData">Type of event that implements IEventData</typeparam>
        /// <param name="eventData">The event instance to evaluate</param>
        /// <returns>true if the event should be published via MassTransit, false otherwise</returns>
        /// <remarks>
        /// <para>
        /// This method allows complex conditional logic to determine publishing strategy.
        /// Can be based on event type, specific event properties, or application context.
        /// </para>
        /// </remarks>
        bool ShouldPublishToMassTransit<TEventData>(TEventData eventData) where TEventData : IEventData;

        /// <summary>
        /// Determines if an event type should be published through MassTransit.
        /// </summary>
        /// <param name="eventType">The event type</param>
        /// <returns>true if events of this type should be published via MassTransit</returns>
        /// <remarks>
        /// <para>
        /// This overload allows optimization through type-based verification without needing
        /// to instantiate the event, useful for static configurations.
        /// </para>
        /// </remarks>
        bool ShouldPublishToMassTransit(Type eventType);
    }
}
