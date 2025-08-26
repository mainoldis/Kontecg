using Kontecg.Dependency;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Defines the contract for managing and coordinating alert message renderers.
    /// This interface provides a centralized point for rendering alerts based on their
    /// display type, delegating the actual rendering to appropriate IAlertMessageRenderer implementations.
    /// </summary>
    /// <remarks>
    /// The IAlertMessageRendererManager interface acts as a facade for the alert rendering system,
    /// abstracting the complexity of selecting and coordinating multiple renderers. It is responsible
    /// for determining which renderer to use based on the alert display type and managing the
    /// rendering process. Implementations are registered as transient dependencies to ensure
    /// proper isolation between requests.
    /// </remarks>
    public interface IAlertMessageRendererManager : ITransientDependency
    {
        /// <summary>
        /// Renders all alerts for the specified display type using the appropriate renderer.
        /// </summary>
        /// <param name="alertDisplayType">The display type for which alerts should be rendered
        /// (e.g., "PageAlert", "Toastr", "Notification").</param>
        /// <returns>
        /// A string representation of all alerts for the specified display type in the format
        /// supported by the corresponding renderer.
        /// </returns>
        /// <remarks>
        /// This method coordinates the rendering process by selecting the appropriate
        /// IAlertMessageRenderer based on the display type and delegating the rendering
        /// to that renderer. The method typically retrieves alerts from the current
        /// AlertManager instance and passes them to the selected renderer for processing.
        /// The returned string can be directly used by the presentation layer to display
        /// the alerts to users in the specified format.
        /// </remarks>
        string Render(string alertDisplayType);
    }
}
