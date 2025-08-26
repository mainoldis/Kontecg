namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Defines the available display types for application alerts and notifications.
    /// This class provides constants for different alert presentation mechanisms
    /// used throughout the application.
    /// </summary>
    /// <remarks>
    /// The alert display types determine how alerts are presented to users in the UI.
    /// Different display types may have different visual characteristics, positioning,
    /// and user interaction patterns.
    /// </remarks>
    public class AlertDisplayType
    {
        /// <summary>
        /// Represents an alert that is displayed as a page-level alert component.
        /// This type typically appears at the top of the current view or page.
        /// </summary>
        /// <remarks>
        /// Page alerts are typically used for important messages that require immediate
        /// user attention and are contextually relevant to the current page content.
        /// </remarks>
        public static string PageAlert = "ViewAlert";

        /// <summary>
        /// Represents an alert that is displayed as a toast notification.
        /// This type typically appears as a temporary overlay in the corner of the screen.
        /// </summary>
        /// <remarks>
        /// Toastr notifications are typically used for non-intrusive messages that
        /// provide feedback for user actions without blocking the interface.
        /// </remarks>
        public static string Toastr = "Toastr";

        /// <summary>
        /// Represents an alert that is displayed as a system notification.
        /// This type may appear in the system tray or notification area.
        /// </summary>
        /// <remarks>
        /// System notifications are typically used for background events or messages
        /// that should be visible even when the application is not in focus.
        /// </remarks>
        public static string Notification = "Notification";
    }
}
