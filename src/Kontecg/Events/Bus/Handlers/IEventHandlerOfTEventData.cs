namespace Kontecg.Events.Bus.Handlers
{
    /// <summary>
    /// Defines the interface for synchronous event handlers that process events of a specific type.
    /// </summary>
    /// <typeparam name="TEventData">
    /// The type of event data that this handler can process. Must implement <see cref="IEventData"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// IEventHandler&lt;TEventData&gt; is the primary interface for implementing synchronous
    /// event handlers in the Kontecg event bus system. It defines a strongly-typed contract
    /// for processing events of a specific type.
    /// </para>
    /// <para>
    /// <strong>Key Characteristics:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Synchronous Processing:</strong> Events are processed synchronously in the calling thread</description></item>
    /// <item><description><strong>Type Safety:</strong> Strongly typed to specific event data types</description></item>
    /// <item><description><strong>Single Responsibility:</strong> Each handler should focus on one type of event</description></item>
    /// <item><description><strong>Stateless Design:</strong> Handlers should generally be stateless for better performance</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Guidelines:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Fast Execution:</strong> Keep processing time minimal to avoid blocking the event bus</description></item>
    /// <item><description><strong>Exception Handling:</strong> Handle exceptions appropriately within the handler</description></item>
    /// <item><description><strong>Thread Safety:</strong> Ensure thread safety if the handler maintains state</description></item>
    /// <item><description><strong>Idempotency:</strong> Design handlers to be idempotent when possible</description></item>
    /// <item><description><strong>Logging:</strong> Include appropriate logging for debugging and monitoring</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Domain Events:</strong> Handle business logic events</description></item>
    /// <item><description><strong>System Events:</strong> Process infrastructure and system events</description></item>
    /// <item><description><strong>Integration Events:</strong> Handle cross-service communication</description></item>
    /// <item><description><strong>Audit Events:</strong> Log and track system activities</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Example Implementation:</strong>
    /// <code>
    /// public class UserCreatedEventHandler : IEventHandler&lt;UserCreatedEventData&gt;
    /// {
    ///     private readonly ILogger _logger;
    ///     private readonly IEmailService _emailService;
    ///     
    ///     public UserCreatedEventHandler(ILogger logger, IEmailService emailService)
    ///     {
    ///         _logger = logger;
    ///         _emailService = emailService;
    ///     }
    ///     
    ///     public void HandleEvent(UserCreatedEventData eventData)
    ///     {
    ///         _logger.LogInformation($"User {eventData.UserName} was created at {eventData.EventTime}");
    ///         
    ///         // Send welcome email
    ///         _emailService.SendWelcomeEmail(eventData.UserEmail, eventData.UserName);
    ///         
    ///         // Update user statistics
    ///         UpdateUserStatistics();
    ///     }
    ///     
    ///     private void UpdateUserStatistics()
    ///     {
    ///         // Implementation for updating statistics
    ///     }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Synchronous handlers block the event bus until completion</description></item>
    /// <item><description>Keep processing time under 100ms for optimal performance</description></item>
    /// <item><description>Consider using async handlers for I/O operations</description></item>
    /// <item><description>Use dependency injection for handler dependencies</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Error Handling:</strong> The event bus will catch exceptions thrown by handlers
    /// and log them, but the event processing will continue with other handlers. Ensure your
    /// handlers handle their own exceptions appropriately.
    /// </para>
    /// </remarks>
    public interface IEventHandler<in TEventData> : IEventHandler
    {
        /// <summary>
        /// Handles the specified event data synchronously.
        /// </summary>
        /// <param name="eventData">
        /// The event data to be processed. Contains all the information about the event
        /// that occurred, including timing and source information.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is called by the event bus when an event of type TEventData is triggered.
        /// The handler should process the event data and perform any necessary business logic,
        /// side effects, or notifications.
        /// </para>
        /// <para>
        /// <strong>Execution Context:</strong>
        /// <list type="bullet">
        /// <item><description>Executes in the same thread as the event trigger</description></item>
        /// <item><description>Blocks the event bus until completion</description></item>
        /// <item><description>Exceptions are caught and logged by the event bus</description></item>
        /// <item><description>Multiple handlers for the same event type are executed sequentially</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Best Practices:</strong>
        /// <list type="bullet">
        /// <item><description>Keep the method execution time minimal</description></item>
        /// <item><description>Handle exceptions appropriately</description></item>
        /// <item><description>Use dependency injection for external services</description></item>
        /// <item><description>Log important operations for debugging</description></item>
        /// <item><description>Avoid blocking operations (use async handlers instead)</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Event Data Access:</strong> The eventData parameter provides access to:
        /// <list type="bullet">
        /// <item><description>Event-specific properties defined in your event data class</description></item>
        /// <item><description>EventTime - when the event occurred</description></item>
        /// <item><description>EventSource - what object triggered the event</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void HandleEvent(TEventData eventData);
    }
}
