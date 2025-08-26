using System.Collections.Generic;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Represents a collection of alert messages that can be displayed to users.
    /// This class extends List&lt;AlertMessage&gt; and provides convenience methods
    /// for creating different types of alerts with predefined configurations.
    /// </summary>
    /// <remarks>
    /// AlertList is designed to simplify the creation and management of user notifications
    /// by providing fluent-style methods for different alert types. It maintains a collection
    /// of AlertMessage objects that can be processed by the alert display system.
    /// </remarks>
    public class AlertList : List<AlertMessage>
    {
        /// <summary>
        /// Adds a new alert message to the collection with the specified parameters.
        /// </summary>
        /// <param name="type">The type of alert (Info, Warning, Danger, Success).</param>
        /// <param name="text">The main message text to display.</param>
        /// <param name="title">Optional title for the alert. If null, a default title based on the type will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, the default display type for the alert type will be used.</param>
        /// <remarks>
        /// This method creates a new AlertMessage instance and adds it to the collection.
        /// The alert will be displayed according to the application's alert display configuration.
        /// </remarks>
        public void Add(AlertType type, string text, string title = null, bool dismissible = true,
            string displayType = null)
        {
            Add(new AlertMessage(type, text, title, dismissible, displayType));
        }

        /// <summary>
        /// Adds an informational alert message to the collection.
        /// </summary>
        /// <param name="text">The main message text to display.</param>
        /// <param name="title">Optional title for the alert. If null, "Information" will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, the default Info display type will be used.</param>
        /// <remarks>
        /// This method creates an informational alert that provides general information or tips to users.
        /// Informational alerts are typically displayed with blue styling and are non-critical.
        /// </remarks>
        public void Info(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Info, text, title, dismissible, displayType));
        }

        /// <summary>
        /// Adds a warning alert message to the collection.
        /// </summary>
        /// <param name="text">The main message text to display.</param>
        /// <param name="title">Optional title for the alert. If null, "Warning" will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, the default Warning display type will be used.</param>
        /// <remarks>
        /// This method creates a warning alert that draws attention to potential issues or important notices.
        /// Warning alerts are typically displayed with yellow/orange styling and indicate caution.
        /// </remarks>
        public void Warning(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Warning, text, title, dismissible, displayType));
        }

        /// <summary>
        /// Adds a danger alert message to the collection.
        /// </summary>
        /// <param name="text">The main message text to display.</param>
        /// <param name="title">Optional title for the alert. If null, "Error" will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, the default Danger display type will be used.</param>
        /// <remarks>
        /// This method creates a danger alert that indicates critical errors or dangerous situations.
        /// Danger alerts are typically displayed with red styling and require immediate attention.
        /// </remarks>
        public void Danger(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Danger, text, title, dismissible, displayType));
        }

        /// <summary>
        /// Adds a success alert message to the collection.
        /// </summary>
        /// <param name="text">The main message text to display.</param>
        /// <param name="title">Optional title for the alert. If null, "Success" will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, the default Success display type will be used.</param>
        /// <remarks>
        /// This method creates a success alert that confirms successful operations or positive outcomes.
        /// Success alerts are typically displayed with green styling and provide positive feedback.
        /// </remarks>
        public void Success(string text, string title = null, bool dismissible = true, string displayType = null)
        {
            Add(new AlertMessage(AlertType.Success, text, title, dismissible, displayType));
        }
    }
}
