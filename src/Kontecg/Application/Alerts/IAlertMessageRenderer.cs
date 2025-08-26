using System.Collections.Generic;
using Kontecg.Dependency;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Defines the contract for rendering alert messages to different output formats.
    /// This interface is implemented by renderers that convert AlertMessage collections
    /// into specific display formats such as HTML, JSON, or other presentation formats.
    /// </summary>
    /// <remarks>
    /// The IAlertMessageRenderer interface is part of the alert system architecture that
    /// separates alert management from alert presentation. Each renderer is responsible
    /// for a specific display type and format. Implementations are registered as transient
    /// dependencies, allowing for multiple renderers to coexist and be selected based on
    /// the required display type or format.
    /// </remarks>
    public interface IAlertMessageRenderer : ITransientDependency
    {
        /// <summary>
        /// Gets the display type that this renderer is responsible for handling.
        /// </summary>
        /// <value>
        /// A string identifier that specifies the display type this renderer supports
        /// (e.g., "PageAlert", "Toastr", "Notification").
        /// </value>
        /// <remarks>
        /// The DisplayType property is used by the alert system to select the appropriate
        /// renderer for a given alert message. This allows for different rendering strategies
        /// based on the alert's display type configuration.
        /// </remarks>
        string DisplayType { get; }

        /// <summary>
        /// Renders a collection of alert messages into the appropriate output format.
        /// </summary>
        /// <param name="alertList">The collection of AlertMessage objects to be rendered.</param>
        /// <returns>
        /// A string representation of the rendered alerts in the format supported by this renderer.
        /// </returns>
        /// <remarks>
        /// This method takes a collection of AlertMessage objects and converts them into
        /// the appropriate output format (e.g., HTML markup, JSON, XML). The renderer is
        /// responsible for applying the correct styling, formatting, and structure based
        /// on the alert types and display configuration. The returned string can be directly
        /// used by the presentation layer to display the alerts to users.
        /// </remarks>
        string Render(List<AlertMessage> alertList);
    }
}
