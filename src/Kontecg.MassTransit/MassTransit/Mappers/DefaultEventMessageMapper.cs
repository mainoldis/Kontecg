using Kontecg.Events.Bus;
using Kontecg.MassTransit.Abstractions;
using Kontecg.MassTransit.Strategies;
using System;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MassTransit.Mappers
{
    /// <summary>
    /// Default mapper that wraps events in EventMessage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation creates a standard wrapper message that includes metadata
    /// useful for distributed processing while preserving original event data.
    /// </para>
    /// </remarks>
    internal class DefaultEventMessageMapper : IEventMessageMapper
    {
        /// <inheritdoc />
        public object MapToMessage<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            var eventMessage = new EventMessage<TEventData>
            {
                EventType = typeof(TEventData).FullName,
                EventSource = eventData.EventSource?.ToString(),
                EventData = eventData,
                Timestamp = eventData.EventTime
            };

            // Add specific metadata if needed
            AddMetadata(eventMessage, eventData);

            return eventMessage;
        }

        /// <summary>
        /// Adds specific metadata to the message based on event type.
        /// </summary>
        /// <param name="eventMessage">Message to enrich</param>
        /// <param name="eventData">Original event</param>
        protected virtual void AddMetadata<TEventData>(EventMessage<TEventData> eventMessage, TEventData eventData)
            where TEventData : IEventData
        {
            // Add type information
            eventMessage.Metadata["AssemblyQualifiedName"] = typeof(TEventData).AssemblyQualifiedName;

            // Add MassTransit attributes if they exist
            var publishAttribute = typeof(TEventData).GetSingleAttributeOfTypeOrBaseTypesOrNull<PublishToDistributedBusAttribute>();
            if (publishAttribute != null)
            {
                eventMessage.Metadata["Priority"] = publishAttribute.Priority;
                if (!string.IsNullOrEmpty(publishAttribute.ExchangeType))
                {
                    eventMessage.Metadata["ExchangeType"] = publishAttribute.ExchangeType;
                }
            }

            // Environment metadata
            eventMessage.Metadata["Environment"] = Environment.MachineName;
            eventMessage.Metadata["ProcessId"] = Environment.ProcessId;
        }
    }
}
