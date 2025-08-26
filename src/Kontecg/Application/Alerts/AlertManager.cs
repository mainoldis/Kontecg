using Kontecg.Dependency;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Manages the collection of alert messages that need to be displayed to users in the application.
    /// This class implements the IAlertManager interface and provides a centralized point for
    /// managing user notifications and alerts.
    /// </summary>
    /// <remarks>
    /// AlertManager is registered as a transient dependency in the dependency injection container,
    /// meaning a new instance is created for each request. This ensures that alerts are isolated
    /// between different user sessions and requests. The class maintains an AlertList collection
    /// that can be populated with various types of alerts (Info, Warning, Danger, Success) and
    /// accessed by the alert display system.
    /// </remarks>
    public class AlertManager : IAlertManager, ITransientDependency
    {
        /// <summary>
        /// Initializes a new instance of the AlertManager class.
        /// </summary>
        /// <remarks>
        /// The constructor creates a new AlertList instance to store alert messages.
        /// This ensures that each AlertManager instance has its own isolated collection
        /// of alerts for the current request or session.
        /// </remarks>
        public AlertManager()
        {
            Alerts = new AlertList();
        }

        /// <summary>
        /// Gets the collection of alert messages that are currently active and need to be displayed.
        /// </summary>
        /// <value>
        /// An AlertList containing all active alert messages that should be presented to users.
        /// </value>
        /// <remarks>
        /// This property provides access to the current set of notifications that need
        /// to be rendered by the alert display system. The collection can be modified by
        /// adding new alerts through the AlertList convenience methods (Info, Warning, Danger, Success)
        /// or by directly adding AlertMessage instances.
        /// </remarks>
        public AlertList Alerts { get; }
    }
}
