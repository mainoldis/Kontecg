using System.Collections.Generic;
using System;

namespace Kontecg.Events.Bus
{
    /// <summary>
    /// Base message for all events distributed through something.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class encapsulates local events by adding metadata necessary for
    /// distribution and processing in distributed systems.
    /// </para>
    /// </remarks>
    public class EventMessage<TEventData> where TEventData : IEventData
    {
        /// <summary>
        /// Unique message identifier.
        /// </summary>
        public Guid MessageId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Timestamp when the message was created.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Original event type.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Source that generated the event.
        /// </summary>
        public string EventSource { get; set; }

        /// <summary>
        /// Serialized event data.
        /// </summary>
        public TEventData EventData { get; set; }

        /// <summary>
        /// Additional event metadata.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Event schema version for compatibility.
        /// </summary>
        public string SchemaVersion { get; set; } = "1.0";

        /// <summary>
        /// Correlation identifier for distributed tracing.
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
