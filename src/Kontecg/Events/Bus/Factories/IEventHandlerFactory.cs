using System;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Events.Bus.Factories
{
    /// <summary>
    /// Defines the interface for factories responsible for creating, managing, and releasing event handlers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// IEventHandlerFactory is a key component in the event bus system that provides fine-grained control
    /// over the lifecycle of event handlers. It follows the Factory pattern to abstract the creation and
    /// management of handlers, enabling advanced scenarios such as dependency injection, resource management,
    /// and custom instantiation logic.
    /// </para>
    /// <para>
    /// <strong>Key Responsibilities:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Handler Creation:</strong> Create new handler instances when needed</description></item>
    /// <item><description><strong>Lifecycle Management:</strong> Control when handlers are created and destroyed</description></item>
    /// <item><description><strong>Resource Management:</strong> Properly dispose of handlers and their resources</description></item>
    /// <item><description><strong>Dependency Injection:</strong> Resolve dependencies for handler construction</description></item>
    /// <item><description><strong>Performance Optimization:</strong> Implement caching, pooling, or lazy loading</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Factory Patterns Supported:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Singleton Factory:</strong> Always returns the same handler instance</description></item>
    /// <item><description><strong>Transient Factory:</strong> Creates a new handler instance for each event</description></item>
    /// <item><description><strong>Scoped Factory:</strong> Creates handlers with specific scope (e.g., per request)</description></item>
    /// <item><description><strong>Pooled Factory:</strong> Reuses handlers from a pool for better performance</description></item>
    /// <item><description><strong>Lazy Factory:</strong> Creates handlers only when first needed</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Scenarios:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Dependency Injection:</strong> Resolve handlers with complex dependencies</description></item>
    /// <item><description><strong>Resource Management:</strong> Control handler lifecycle and disposal</description></item>
    /// <item><description><strong>Performance Optimization:</strong> Implement caching or pooling strategies</description></item>
    /// <item><description><strong>Testing:</strong> Provide mock handlers for unit testing</description></item>
    /// <item><description><strong>Configuration:</strong> Create handlers based on configuration settings</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Guidelines:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Thread Safety:</strong> Ensure factory methods are thread-safe</description></item>
    /// <item><description><strong>Resource Cleanup:</strong> Properly dispose of handlers and their resources</description></item>
    /// <item><description><strong>Error Handling:</strong> Handle creation failures gracefully</description></item>
    /// <item><description><strong>Performance:</strong> Optimize for the expected usage patterns</description></item>
    /// <item><description><strong>Logging:</strong> Include appropriate logging for debugging</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Example Implementation:</strong>
    /// <code>
    /// public class UserEventHandlerFactory : IEventHandlerFactory
    /// {
    ///     private readonly IServiceProvider _serviceProvider;
    ///     private readonly ILogger _logger;
    ///     
    ///     public UserEventHandlerFactory(IServiceProvider serviceProvider, ILogger logger)
    ///     {
    ///         _serviceProvider = serviceProvider;
    ///         _logger = logger;
    ///     }
    ///     
    ///     public IEventHandler GetHandler()
    ///     {
    ///         try
    ///         {
    ///             // Resolve handler with dependencies from DI container
    ///             var handler = _serviceProvider.GetRequiredService&lt;UserCreatedEventHandler&gt;();
    ///             _logger.LogDebug("Created UserCreatedEventHandler instance");
    ///             return handler;
    ///         }
    ///         catch (Exception ex)
    ///         {
    ///             _logger.LogError(ex, "Failed to create UserCreatedEventHandler");
    ///             throw;
    ///         }
    ///     }
    ///     
    ///     public Type GetHandlerType()
    ///     {
    ///         return typeof(UserCreatedEventHandler);
    ///     }
    ///     
    ///     public void ReleaseHandler(IEventHandler handler)
    ///     {
    ///         if (handler is IDisposable disposable)
    ///         {
    ///             disposable.Dispose();
    ///             _logger.LogDebug("Disposed UserCreatedEventHandler instance");
    ///         }
    ///     }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Integration with Event Bus:</strong> The event bus uses factories to:
    /// <list type="bullet">
    /// <item><description>Create handlers when events are triggered</description></item>
    /// <item><description>Manage handler lifecycle and resources</description></item>
    /// <item><description>Provide type information for registration</description></item>
    /// <item><description>Enable advanced dependency injection scenarios</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Factory creation should be fast and lightweight</description></item>
    /// <item><description>Handler creation should be optimized for the expected usage</description></item>
    /// <item><description>Consider caching or pooling for frequently used handlers</description></item>
    /// <item><description>Resource cleanup should be efficient and thorough</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// Creates and returns a new event handler instance.
        /// </summary>
        /// <returns>
        /// A new instance of an event handler that implements <see cref="IEventHandler"/>.
        /// The factory is responsible for creating the handler with all necessary dependencies
        /// and configuration.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is called by the event bus when an event is triggered and a handler
        /// instance is needed. The factory should create a fully configured handler instance
        /// that is ready to process events.
        /// </para>
        /// <para>
        /// <strong>Creation Responsibilities:</strong>
        /// <list type="bullet">
        /// <item><description>Instantiate the handler with all required dependencies</description></item>
        /// <item><description>Configure the handler with any necessary settings</description></item>
        /// <item><description>Ensure the handler is in a valid state for processing events</description></item>
        /// <item><description>Handle any creation errors and provide meaningful exceptions</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance Considerations:</strong>
        /// <list type="bullet">
        /// <item><description>Creation should be as fast as possible</description></item>
        /// <item><description>Consider caching or pooling for expensive operations</description></item>
        /// <item><description>Lazy initialization for heavy dependencies</description></item>
        /// <item><description>Minimize allocations during creation</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Error Handling:</strong> If handler creation fails, the factory should
        /// throw an appropriate exception. The event bus will catch and log these exceptions,
        /// but event processing will continue with other handlers.
        /// </para>
        /// <para>
        /// <strong>Thread Safety:</strong> This method may be called from multiple threads
        /// concurrently, so the factory implementation should be thread-safe.
        /// </para>
        /// </remarks>
        IEventHandler GetHandler();

        /// <summary>
        /// Gets the type of the handler without creating an instance.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/> of the handler that this factory creates. This is used
        /// by the event bus for type checking, registration validation, and debugging purposes.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides type information about the handler without the overhead of
        /// creating an actual instance. It's useful for validation, debugging, and type-based
        /// operations in the event bus.
        /// </para>
        /// <para>
        /// <strong>Usage by Event Bus:</strong>
        /// <list type="bullet">
        /// <item><description>Validate handler types during registration</description></item>
        /// <item><description>Check type compatibility with event types</description></item>
        /// <item><description>Provide type information for logging and debugging</description></item>
        /// <item><description>Enable type-based handler filtering and management</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method should be very fast and should not
        /// perform any expensive operations or allocations.
        /// </para>
        /// <para>
        /// <strong>Consistency:</strong> The returned type should always be the same for
        /// a given factory instance and should match the type of handlers created by GetHandler().
        /// </para>
        /// </remarks>
        Type GetHandlerType();

        /// <summary>
        /// Releases and cleans up resources associated with an event handler.
        /// </summary>
        /// <param name="handler">
        /// The event handler instance to be released. This is the same instance that was
        /// previously created by the GetHandler() method.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is called by the event bus when a handler instance is no longer needed.
        /// The factory should perform any necessary cleanup operations, such as disposing
        /// of resources, returning objects to pools, or releasing references.
        /// </para>
        /// <para>
        /// <strong>Cleanup Responsibilities:</strong>
        /// <list type="bullet">
        /// <item><description>Dispose of any disposable resources held by the handler</description></item>
        /// <item><description>Return pooled objects to their pools</description></item>
        /// <item><description>Release any cached or shared resources</description></item>
        /// <item><description>Clear any references to prevent memory leaks</description></item>
        /// <item><description>Log cleanup operations for debugging</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Handler Validation:</strong> The factory should validate that the handler
        /// parameter is of the correct type and was created by this factory. This helps
        /// prevent resource management issues and provides better error messages.
        /// </para>
        /// <para>
        /// <strong>Error Handling:</strong> Cleanup operations should be robust and should
        /// not throw exceptions that could affect the event bus operation. Any cleanup
        /// errors should be logged but not propagated.
        /// </para>
        /// <para>
        /// <strong>Thread Safety:</strong> This method may be called from multiple threads
        /// concurrently, so the cleanup implementation should be thread-safe.
        /// </para>
        /// <para>
        /// <strong>Idempotency:</strong> The method should be idempotent - calling it multiple
        /// times on the same handler should be safe and should not cause issues.
        /// </para>
        /// </remarks>
        void ReleaseHandler(IEventHandler handler);
    }
}
