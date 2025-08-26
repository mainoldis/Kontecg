using JetBrains.Annotations;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Represents a single alert message that can be displayed to users in the application.
    /// This class encapsulates all the information needed to render an alert, including
    /// its content, type, display configuration, and user interaction options.
    /// </summary>
    /// <remarks>
    /// AlertMessage objects are created by the alert system and processed by alert renderers
    /// to display appropriate user notifications. The class provides validation for required
    /// fields and default values for optional properties.
    /// </remarks>
    public class AlertMessage
    {
        private string _text;

        /// <summary>
        /// Initializes a new instance of the AlertMessage class with the specified parameters.
        /// </summary>
        /// <param name="type">The type of alert that determines its visual appearance and severity.</param>
        /// <param name="text">The main message text to display. Cannot be null, empty, or whitespace.</param>
        /// <param name="title">Optional title for the alert. If null, a default title based on the type will be used.</param>
        /// <param name="dismissible">Whether the alert can be dismissed by the user. Default is true.</param>
        /// <param name="displayType">Optional display type override. If null, PageAlert will be used as default.</param>
        /// <exception cref="ArgumentException">Thrown when the text parameter is null, empty, or whitespace.</exception>
        /// <remarks>
        /// The constructor validates the text parameter using the Check utility class to ensure
        /// that all alert messages have meaningful content. The display type defaults to PageAlert
        /// if not specified.
        /// </remarks>
        public AlertMessage(AlertType type, [NotNull] string text, string title = null, bool dismissible = true,
            string displayType = null)
        {
            Type = type;
            Text = Check.NotNullOrWhiteSpace(text, nameof(text));
            Title = title;
            Dismissible = dismissible;
            DisplayType = displayType ?? AlertDisplayType.PageAlert;
        }

        /// <summary>
        /// Gets or sets the main message text of the alert.
        /// </summary>
        /// <value>
        /// The message text that will be displayed to the user. Cannot be null, empty, or whitespace.
        /// </value>
        /// <exception cref="ArgumentException">Thrown when attempting to set a null, empty, or whitespace value.</exception>
        /// <remarks>
        /// The text property is validated using Check.NotNullOrWhiteSpace to ensure that
        /// all alert messages contain meaningful content for users.
        /// </remarks>
        [NotNull]
        public string Text
        {
            get => _text;
            set => _text = Check.NotNullOrWhiteSpace(value, nameof(value));
        }

        /// <summary>
        /// Gets or sets the type of alert that determines its visual appearance and severity.
        /// </summary>
        /// <value>
        /// An AlertType value that indicates whether this is an Info, Warning, Danger, or Success alert.
        /// </value>
        /// <remarks>
        /// The alert type influences the color scheme, icon, and overall styling of the alert
        /// when it is rendered in the user interface.
        /// </remarks>
        public AlertType Type { get; set; }

        /// <summary>
        /// Gets or sets the optional title for the alert.
        /// </summary>
        /// <value>
        /// The title text that will be displayed as the alert header. Can be null if no title is needed.
        /// </value>
        /// <remarks>
        /// If no title is provided, the alert renderer may use a default title based on the alert type
        /// or display the alert without a title section.
        /// </remarks>
        [CanBeNull] public string Title { get; set; }

        /// <summary>
        /// Gets or sets whether the alert can be dismissed by the user.
        /// </summary>
        /// <value>
        /// True if the user can dismiss the alert; otherwise, false. Default is true.
        /// </value>
        /// <remarks>
        /// Dismissible alerts typically include a close button or allow clicking outside the alert
        /// to dismiss it. Non-dismissible alerts may require specific user actions to remove them.
        /// </remarks>
        public bool Dismissible { get; set; }

        /// <summary>
        /// Gets or sets the display type that determines how the alert is presented to the user.
        /// </summary>
        /// <value>
        /// A string indicating the display mechanism (e.g., "PageAlert", "Toastr", "Notification").
        /// Default is "PageAlert".
        /// </value>
        /// <remarks>
        /// The display type controls whether the alert appears as a page-level alert, toast notification,
        /// or system notification. This allows for different presentation strategies based on
        /// the alert's importance and context.
        /// </remarks>
        public string DisplayType { get; set; }
    }
}
