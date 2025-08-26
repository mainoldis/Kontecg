namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Defines the severity levels and types of alerts that can be displayed in the application.
    /// This enumeration is used to categorize alerts based on their importance and visual styling.
    /// </summary>
    /// <remarks>
    /// The alert type determines the visual appearance, color scheme, and icon used to display
    /// the alert to users. Each type conveys a different level of urgency and context.
    /// </remarks>
    public enum AlertType
    {
        /// <summary>
        /// Represents a successful operation or positive outcome.
        /// Typically displayed with green styling and a checkmark icon.
        /// </summary>
        /// <remarks>
        /// Success alerts are used to confirm that an operation completed successfully
        /// or to provide positive feedback to user actions.
        /// </remarks>
        Success,

        /// <summary>
        /// Represents a critical error or dangerous situation that requires immediate attention.
        /// Typically displayed with red styling and an error icon.
        /// </summary>
        /// <remarks>
        /// Danger alerts are used for critical errors, security issues, or situations
        /// that may cause data loss or system instability.
        /// </remarks>
        Danger,

        /// <summary>
        /// Represents a warning or cautionary message that should be noted.
        /// Typically displayed with yellow/orange styling and a warning icon.
        /// </summary>
        /// <remarks>
        /// Warning alerts are used for potential issues, deprecated features, or
        /// situations that may require user attention but are not critical.
        /// </remarks>
        Warning,

        /// <summary>
        /// Represents an informational message or general notification.
        /// Typically displayed with blue styling and an information icon.
        /// </summary>
        /// <remarks>
        /// Info alerts are used for general information, tips, or non-critical
        /// notifications that provide context or guidance to users.
        /// </remarks>
        Info
    }
}
