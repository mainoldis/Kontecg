using Kontecg.Events.Bus;
using Kontecg.MassTransit.Abstractions;
using System.Collections.Generic;
using System;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MassTransit.Strategies
{
    /// <summary>
    /// Attribute-based strategy for determining which events to publish to MassTransit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation uses decorator attributes to declarative configure
    /// event publishing behavior. It's the recommended strategy for most
    /// use cases due to its simplicity and clarity.
    /// </para>
    /// </remarks>
    public class AttributeBasedPublishingStrategy : IEventPublishingStrategy
    {
        /// <summary>
        /// Cache of already evaluated types for performance optimization.
        /// </summary>
        private readonly Dictionary<Type, bool> _typeCache = new();
        private readonly object _cacheLock = new object();

        /// <inheritdoc />
        public bool ShouldPublishToMassTransit<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            if (eventData == null) return false;
            return ShouldPublishToMassTransit(eventData.GetType());
        }

        /// <inheritdoc />
        public bool ShouldPublishToMassTransit(Type eventType)
        {
            if (eventType == null) return false;

            // Check cache first for performance optimization
            lock (_cacheLock)
            {
                if (_typeCache.TryGetValue(eventType, out bool cachedResult))
                {
                    return cachedResult;
                }
            }

            // Evaluate attributes
            bool shouldPublish = EvaluateEventType(eventType);

            // Store in cache
            lock (_cacheLock)
            {
                _typeCache[eventType] = shouldPublish;
            }

            return shouldPublish;
        }

        /// <summary>
        /// Evaluates if an event type should be published based on its attributes.
        /// </summary>
        /// <param name="eventType">Event type to evaluate</param>
        /// <returns>true if it should be published to MassTransit</returns>
        private bool EvaluateEventType(Type eventType)
        {
            // If has LocalOnly attribute, don't publish
            if (eventType.GetSingleAttributeOrNull<LocalOnlyEventAttribute>() != null)
            {
                return false;
            }

            // If has PublishToMassTransit attribute, publish
            if (eventType.GetSingleAttributeOrNull<PublishToDistributedBusAttribute>() != null)
            {
                return true;
            }

            // Check class hierarchy for attribute inheritance
            Type currentType = eventType.BaseType;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.GetSingleAttributeOrNull<PublishToDistributedBusAttribute>() != null)
                {
                    return true;
                }

                if (currentType.GetSingleAttributeOrNull<LocalOnlyEventAttribute>() != null)
                {
                    return false;
                }

                currentType = currentType.BaseType;
            }

            // By default, don't publish (conservative behavior)
            return false;
        }
    }
}
