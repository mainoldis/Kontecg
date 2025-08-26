using System;
using Kontecg.Timing;

namespace Kontecg.Events.Bus
{
    /// <summary>
    /// Provides a base implementation of <see cref="IEventData"/> for event data classes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// EventData is an abstract base class that implements the <see cref="IEventData"/>
    /// interface and provides common functionality for all event data classes in the
    /// Kontecg event bus system. It automatically handles event timing and provides
    /// a foundation for custom event implementations.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Automatic Timing:</strong> EventTime is automatically set to the current time</description></item>
    /// <item><description><strong>Serialization Support:</strong> Marked as serializable for persistence and transmission</description></item>
    /// <item><description><strong>Base Implementation:</strong> Provides common IEventData implementation</description></item>
    /// <item><description><strong>Extensibility:</strong> Designed for inheritance and customization</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Inheritance Pattern:</strong> Custom event data classes should inherit
    /// from this class to get the common functionality while adding their specific
    /// event data properties.
    /// </para>
    /// <para>
    /// <strong>Usage Example:</strong>
    /// <code>
    /// public class UserCreatedEventData : EventData
    /// {
    ///     public string UserName { get; set; }
    ///     public string UserEmail { get; set; }
    ///     public DateTime CreatedAt { get; set; }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Clock Integration:</strong> The class uses <see cref="Clock.Now"/> to
    /// ensure consistent timing across the application, which is particularly important
    /// in distributed systems and for audit purposes.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class EventData : IEventData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventData"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The constructor automatically sets the <see cref="EventTime"/> property to
        /// the current time using <see cref="Clock.Now"/>. This ensures that all events
        /// have accurate timing information from the moment they are created.
        /// </para>
        /// <para>
        /// <strong>Timing Precision:</strong> Using <see cref="Clock.Now"/> instead of
        /// <see cref="DateTime.Now"/> ensures consistent timing across the application,
        /// which is especially important in scenarios where time synchronization is critical.
        /// </para>
        /// <para>
        /// <strong>Initialization:</strong> The <see cref="EventSource"/> property is
        /// left as null by default and should be set by the code that creates the event
        /// if the event source is known and relevant.
        /// </para>
        /// </remarks>
        protected EventData()
        {
            EventTime = Clock.Now;
        }

        /// <summary>
        /// Gets or sets the time when the event occurred.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is automatically set to the current time when the event is
        /// created using the <see cref="Clock.Now"/> method. It provides a precise
        /// timestamp of when the event occurred.
        /// </para>
        /// <para>
        /// <strong>Automatic Setting:</strong> The event time is set in the constructor
        /// and typically should not be modified after the event is created, as it
        /// represents the actual occurrence time of the event.
        /// </para>
        /// <para>
        /// <strong>Clock Consistency:</strong> Using <see cref="Clock.Now"/> ensures
        /// that all events in the system use the same time source, which is important
        /// for event ordering and timing analysis.
        /// </para>
        /// </remarks>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Gets or sets the object that triggered the event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property identifies the object that caused the event to occur. It is
        /// initially set to null and should be set by the code that creates and triggers
        /// the event if the event source is known and relevant.
        /// </para>
        /// <para>
        /// <strong>Event Source Identification:</strong> The event source can be any
        /// object that is relevant to understanding the context of the event. Common
        /// examples include:
        /// <list type="bullet">
        /// <item><description>Domain entities (User, Order, Product)</description></item>
        /// <item><description>Services that perform operations (UserService, OrderService)</description></item>
        /// <item><description>System components (BackgroundJob, ScheduledTask)</description></item>
        /// <item><description>External systems or integrations</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Optional Property:</strong> This property is optional and can remain
        /// null if the event source is not relevant or not available. However, providing
        /// the event source is recommended for better event traceability and debugging.
        /// </para>
        /// <para>
        /// <strong>Usage in Event Creation:</strong> When creating events, you can set
        /// this property to provide context:
        /// <code>
        /// var eventData = new UserCreatedEventData
        /// {
        ///     EventSource = this, // The service or component creating the event
        ///     UserName = "John Doe",
        ///     UserEmail = "john@example.com"
        /// };
        /// </code>
        /// </para>
        /// </remarks>
        public object EventSource { get; set; }
    }
}
