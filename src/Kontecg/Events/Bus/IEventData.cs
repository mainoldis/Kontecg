using System;

namespace Kontecg.Events.Bus
{
    /// <summary>
    /// Defines the base interface for all event data classes in the Kontecg event bus system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// IEventData is the fundamental interface that all event data classes must implement
    /// to participate in the event bus system. It provides the basic metadata that every
    /// event should contain, including timing information and source identification.
    /// </para>
    /// <para>
    /// <strong>Event Data Structure:</strong> Every event in the system contains:
    /// <list type="bullet">
    /// <item><description><strong>EventTime:</strong> When the event occurred</description></item>
    /// <item><description><strong>EventSource:</strong> What object triggered the event</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Guidelines:</strong>
    /// <list type="bullet">
    /// <item><description>All custom event data classes should implement this interface</description></item>
    /// <item><description>Consider inheriting from <see cref="EventData"/> for common implementations</description></item>
    /// <item><description>EventTime should be set when the event is created</description></item>
    /// <item><description>EventSource should identify the object that caused the event</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// <list type="bullet">
    /// <item><description>Domain events for business logic notifications</description></item>
    /// <item><description>System events for infrastructure notifications</description></item>
    /// <item><description>Integration events for cross-service communication</description></item>
    /// <item><description>Audit events for tracking system activities</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IEventData
    {
        /// <summary>
        /// Gets or sets the time when the event occurred.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property indicates when the event was triggered. It is typically set
        /// automatically when the event is created and should represent the exact moment
        /// when the event occurred.
        /// </para>
        /// <para>
        /// <strong>Timing Precision:</strong> The event time should be as precise as
        /// possible to ensure accurate event ordering and timing analysis.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This timestamp is useful for:
        /// <list type="bullet">
        /// <item><description>Event ordering and sequencing</description></item>
        /// <item><description>Audit trails and compliance</description></item>
        /// <item><description>Performance analysis and monitoring</description></item>
        /// <item><description>Debugging and troubleshooting</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Default Behavior:</strong> When inheriting from <see cref="EventData"/>,
        /// this property is automatically set to the current time when the event is created.
        /// </para>
        /// </remarks>
        DateTime EventTime { get; set; }

        /// <summary>
        /// Gets or sets the object that triggered the event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property identifies the object that caused the event to occur. It provides
        /// context about the origin of the event and can be useful for event handlers
        /// that need to know what triggered the event.
        /// </para>
        /// <para>
        /// <strong>Event Source Types:</strong> The event source can be:
        /// <list type="bullet">
        /// <item><description>A domain entity (e.g., User, Order, Product)</description></item>
        /// <item><description>A service or component (e.g., UserService, OrderProcessor)</description></item>
        /// <item><description>A system component (e.g., BackgroundJob, ScheduledTask)</description></item>
        /// <item><description>Any object that can be identified as the event origin</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Optional Property:</strong> This property is optional and can be null
        /// if the event source is not relevant or not available. However, providing
        /// the event source is recommended for better event traceability.
        /// </para>
        /// <para>
        /// <strong>Usage in Handlers:</strong> Event handlers can use this property to:
        /// <list type="bullet">
        /// <item><description>Determine the context of the event</description></item>
        /// <item><description>Apply conditional logic based on the source</description></item>
        /// <item><description>Log or audit the event origin</description></item>
        /// <item><description>Implement source-specific processing</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        object EventSource { get; set; }
    }
}
