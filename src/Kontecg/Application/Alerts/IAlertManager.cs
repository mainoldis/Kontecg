namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Defines the contract for managing application alerts and user notifications.
    /// This interface provides access to the collection of alerts that need to be displayed
    /// to users in the application.
    /// </summary>
    /// <remarks>
    /// The IAlertManager interface is part of the alert system architecture that separates
    /// alert management from alert rendering. Implementations of this interface typically
    /// maintain a collection of AlertMessage objects and provide methods for adding,
    /// removing, and managing alert lifecycle.
    /// </remarks>
    public interface IAlertManager
    {
        /// <summary>
        /// Gets the collection of alert messages that are currently active and need to be displayed.
        /// </summary>
        /// <value>
        /// An AlertList containing all active alert messages that should be presented to users.
        /// </value>
        /// <remarks>
        /// The Alerts property provides access to the current set of notifications that need
        /// to be rendered by the alert display system. The collection may be modified by
        /// adding new alerts or removing dismissed ones.
        /// </remarks>
        AlertList Alerts { get; }
    }
}
