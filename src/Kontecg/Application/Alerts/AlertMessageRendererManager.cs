using System.Collections.Generic;
using System.Linq;

namespace Kontecg.Application.Alerts
{
    /// <summary>
    /// Manages the rendering of alert messages using registered renderers.
    /// </summary>
    /// <remarks>
    /// This manager selects the appropriate <see cref="IAlertMessageRenderer"/> based on the alert display type and delegates the rendering process.
    /// </remarks>
    public class AlertMessageRendererManager : IAlertMessageRendererManager
    {
        private readonly IAlertManager _alertManager;
        private readonly IEnumerable<IAlertMessageRenderer> _alertMessageRenderers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertMessageRendererManager"/> class.
        /// </summary>
        /// <param name="alertMessageRenderers">A collection of alert message renderers.</param>
        /// <param name="AlertManager">The alert manager instance.</param>
        public AlertMessageRendererManager(
            IEnumerable<IAlertMessageRenderer> alertMessageRenderers,
            IAlertManager AlertManager)
        {
            _alertMessageRenderers = alertMessageRenderers;
            _alertManager = AlertManager;
        }

        /// <summary>
        /// Renders alert messages for the specified display type.
        /// </summary>
        /// <param name="alertDisplayType">The display type of the alert.</param>
        /// <returns>The rendered alert message as a string, or an empty string if no alerts match the display type.</returns>
        public string Render(string alertDisplayType)
        {
            if (_alertManager.Alerts.All(a => a.DisplayType != alertDisplayType))
            {
                return "";
            }

            IAlertMessageRenderer alertMessageRenderer =
                _alertMessageRenderers.FirstOrDefault(x => x.DisplayType == alertDisplayType);

            return alertMessageRenderer?.Render(_alertManager.Alerts.Where(a => a.DisplayType == alertDisplayType)
                .ToList());
        }
    }
}
