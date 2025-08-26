using System.Threading.Tasks;

namespace Kontecg.Events.Bus.Handlers
{
    /// <summary>
    /// Defines the interface for asynchronous event handlers that process events of a specific type.
    /// </summary>
    /// <typeparam name="TEventData">
    /// The type of event data that this handler can process. Must implement <see cref="IEventData"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// IAsyncEventHandler&lt;TEventData&gt; is the interface for implementing asynchronous
    /// event handlers in the Kontecg event bus system. It defines a strongly-typed contract
    /// for processing events of a specific type without blocking the calling thread.
    /// </para>
    /// <para>
    /// <strong>Key Characteristics:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Asynchronous Processing:</strong> Events are processed asynchronously without blocking</description></item>
    /// <item><description><strong>Non-Blocking:</strong> The event bus continues processing other events</description></item>
    /// <item><description><strong>Type Safety:</strong> Strongly typed to specific event data types</description></item>
    /// <item><description><strong>Scalability:</strong> Better performance for I/O operations and long-running tasks</description></item>
    /// <item><description><strong>Error Handling:</strong> Exceptions are captured in the returned Task</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>When to Use Async Handlers:</strong>
    /// <list type="bullet">
    /// <item><description><strong>I/O Operations:</strong> Database queries, file operations, network calls</description></item>
    /// <item><description><strong>External Services:</strong> API calls, email sending, SMS notifications</description></item>
    /// <item><description><strong>Long-Running Tasks:</strong> Complex calculations, data processing</description></item>
    /// <item><description><strong>Resource-Intensive Operations:</strong> Image processing, report generation</description></item>
    /// <item><description><strong>Parallel Processing:</strong> When multiple operations can be performed concurrently</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Guidelines:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Async/Await Pattern:</strong> Use async/await consistently throughout the method</description></item>
    /// <item><description><strong>Exception Handling:</strong> Handle exceptions appropriately and return failed tasks</description></item>
    /// <item><description><strong>Cancellation Support:</strong> Consider supporting cancellation tokens for long operations</description></item>
    /// <item><description><strong>Logging:</strong> Include appropriate logging for debugging and monitoring</description></item>
    /// <item><description><description><strong>Idempotency:</strong> Design handlers to be idempotent when possible</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Example Implementation:</strong>
    /// <code>
    /// public class UserCreatedAsyncEventHandler : IAsyncEventHandler&lt;UserCreatedEventData&gt;
    /// {
    ///     private readonly ILogger _logger;
    ///     private readonly IEmailService _emailService;
    ///     private readonly IUserRepository _userRepository;
    ///     
    ///     public UserCreatedAsyncEventHandler(
    ///         ILogger logger, 
    ///         IEmailService emailService, 
    ///         IUserRepository userRepository)
    ///     {
    ///         _logger = logger;
    ///         _emailService = emailService;
    ///         _userRepository = userRepository;
    ///     }
    ///     
    ///     public async Task HandleEventAsync(UserCreatedEventData eventData)
    ///     {
    ///         try
    ///         {
    ///             _logger.LogInformation($"Processing user creation for {eventData.UserName}");
    ///             
    ///             // Send welcome email asynchronously
    ///             var emailTask = _emailService.SendWelcomeEmailAsync(
    ///                 eventData.UserEmail, 
    ///                 eventData.UserName);
    ///             
    ///             // Update user statistics asynchronously
    ///             var statsTask = UpdateUserStatisticsAsync(eventData.UserId);
    ///             
    ///             // Wait for both operations to complete
    ///             await Task.WhenAll(emailTask, statsTask);
    ///             
    ///             _logger.LogInformation($"Successfully processed user creation for {eventData.UserName}");
    ///         }
    ///         catch (Exception ex)
    ///         {
    ///             _logger.LogError(ex, $"Error processing user creation for {eventData.UserName}");
    ///             throw; // Re-throw to let the event bus handle it
    ///         }
    ///     }
    ///     
    ///     private async Task UpdateUserStatisticsAsync(int userId)
    ///     {
    ///         // Async implementation for updating statistics
    ///         await _userRepository.UpdateStatisticsAsync(userId);
    ///     }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Performance Benefits:</strong>
    /// <list type="bullet">
    /// <item><description>Non-blocking event processing</description></item>
    /// <item><description>Better resource utilization</description></item>
    /// <item><description>Improved application responsiveness</description></item>
    /// <item><description>Support for concurrent operations</description></item>
    /// <item><description>Better scalability for high-throughput scenarios</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Error Handling:</strong> The event bus will await the returned Task and catch
    /// any exceptions that occur during execution. Failed tasks are logged, but the event
    /// processing continues with other handlers. Ensure your handlers handle exceptions
    /// appropriately and return failed tasks when necessary.
    /// </para>
    /// <para>
    /// <strong>Execution Context:</strong> Async handlers are executed in the background,
    /// allowing the event bus to continue processing other events without waiting for
    /// the async operation to complete. This provides better performance and responsiveness
    /// for the overall application.
    /// </para>
    /// </remarks>
    public interface IAsyncEventHandler<in TEventData> : IEventHandler
    {
        /// <summary>
        /// Handles the specified event data asynchronously.
        /// </summary>
        /// <param name="eventData">
        /// The event data to be processed. Contains all the information about the event
        /// that occurred, including timing and source information.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of processing the event.
        /// The task completes when the event processing is finished.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is called by the event bus when an event of type TEventData is triggered.
        /// The handler should process the event data asynchronously and perform any necessary
        /// business logic, side effects, or notifications without blocking the calling thread.
        /// </para>
        /// <para>
        /// <strong>Execution Context:</strong>
        /// <list type="bullet">
        /// <item><description>Executes asynchronously without blocking the event bus</description></item>
        /// <item><description>Returns immediately with a Task</description></item>
        /// <item><description>Exceptions are captured in the returned Task</description></item>
        /// <item><description>Multiple handlers for the same event type can execute concurrently</description></item>
        /// <item><description>Uses the default task scheduler for execution</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Async/Await Best Practices:</strong>
        /// <list type="bullet">
        /// <item><description>Use async/await consistently throughout the method</description></item>
        /// <item><description>Avoid blocking operations (Task.Wait, Task.Result)</description></item>
        /// <item><description>Use ConfigureAwait(false) for library code</description></item>
        /// <item><description>Handle exceptions with try-catch blocks</description></item>
        /// <item><description>Return failed tasks for error conditions</description></item>
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
        /// <para>
        /// <strong>Task Completion:</strong> The returned Task should complete when all
        /// asynchronous operations are finished. This allows the event bus to properly
        /// handle completion and any exceptions that occur during processing.
        /// </para>
        /// </remarks>
        Task HandleEventAsync(TEventData eventData);
    }
}
