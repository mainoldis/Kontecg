namespace Kontecg.ExceptionHandling
{
    /// <summary>
    /// Provides configuration options for the exception handling system.
    /// This class defines various settings that control how exceptions are processed,
    /// logged, and handled throughout the application.
    /// </summary>
    /// <remarks>
    /// ExceptionHandlingConfiguration is used to configure the behavior of the exception
    /// handling infrastructure. It provides settings for enabling/disabling exception
    /// handling, controlling the level of detail in error messages, managing domain
    /// event generation for exceptions, and determining whether handled exceptions
    /// should be propagated. The configuration is typically set during application
    /// startup and can be modified at runtime if needed. This class implements
    /// IExceptionHandlingConfiguration to provide a standardized interface for
    /// accessing exception handling settings.
    /// </remarks>
    internal class ExceptionHandlingConfiguration : IExceptionHandlingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the ExceptionHandlingConfiguration class
        /// with default settings.
        /// </summary>
        /// <remarks>
        /// The constructor sets up default values for all configuration options:
        /// - Exception handling is enabled by default
        /// - Detailed exceptions are not sent to support by default (for security)
        /// - Domain events are triggered for exceptions by default
        /// - Handled exceptions are propagated by default
        /// These defaults provide a good balance between functionality and security.
        /// </remarks>
        public ExceptionHandlingConfiguration()
        {
            IsEnabled = true;
            SendDetailedExceptionsToSupport = false;
            TriggerDomainEvents = true;
            PropagatedHandledExceptions = true;
        }

        /// <summary>
        /// Gets or sets whether the exception handling system is enabled.
        /// </summary>
        /// <value>
        /// True if exception handling is enabled; otherwise, false. Default is true.
        /// </value>
        /// <remarks>
        /// When enabled, the exception handling system will intercept and process
        /// exceptions according to the configured settings. When disabled, exceptions
        /// will be allowed to propagate normally without any special handling.
        /// This setting is useful for debugging scenarios where you want to see
        /// the original exception behavior without interference from the handling system.
        /// </remarks>
        /// <inheritdoc />
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether handled exceptions should be propagated to the caller.
        /// </summary>
        /// <value>
        /// True if handled exceptions should be propagated; otherwise, false. Default is true.
        /// </value>
        /// <remarks>
        /// When enabled, exceptions that are handled by the exception handling system
        /// will still be propagated to the calling code. This allows calling code to
        /// implement its own exception handling logic while still benefiting from the
        /// centralized exception processing. When disabled, handled exceptions are
        /// completely suppressed and not re-thrown.
        /// </remarks>
        /// <inheritdoc />
        public bool PropagatedHandledExceptions { get; set; }

        /// <summary>
        /// Gets or sets whether detailed exception information should be sent to support.
        /// </summary>
        /// <value>
        /// True if detailed exceptions should be sent to support; otherwise, false. Default is false.
        /// </value>
        /// <remarks>
        /// This setting controls whether sensitive exception details (such as stack traces,
        /// internal error messages, and system information) are included in error reports
        /// sent to support or logging systems. For security reasons, this is disabled by
        /// default to prevent information disclosure. It should only be enabled in
        /// controlled environments where detailed error information is necessary for
        /// debugging and support purposes.
        /// </remarks>
        /// <inheritdoc />
        public bool SendDetailedExceptionsToSupport { get; set; }

        /// <summary>
        /// Gets or sets whether domain events should be triggered when exceptions occur.
        /// </summary>
        /// <value>
        /// True if domain events should be triggered for exceptions; otherwise, false. Default is true.
        /// </value>
        /// <remarks>
        /// When enabled, the exception handling system will trigger domain events
        /// when exceptions occur. These events can be used by other parts of the
        /// system to respond to exceptions, such as sending notifications, updating
        /// monitoring systems, or performing cleanup operations. Domain events provide
        /// a loosely coupled way to handle exception side effects without tightly
        /// coupling the exception handling logic to specific response actions.
        /// </remarks>
        /// <inheritdoc />
        public bool TriggerDomainEvents { get; set; }
    }
}
