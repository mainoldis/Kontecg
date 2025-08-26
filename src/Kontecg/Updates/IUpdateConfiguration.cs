using System;
using Kontecg.Configuration.Startup;

namespace Kontecg.Updates
{
    public interface IUpdateConfiguration
    {
        /// <summary>
        ///     Enables/Disables update system.
        ///     Default: true.
        /// </summary>
        bool IsUpdateCheckEnabled { get; set; }

        /// <summary>
        ///     Check for new updates only once at application start.
        ///     Default: false.
        /// </summary>
        bool CheckForUpdatesAtStartup { get; set; }

        /// <summary>
        ///     List of update sources.
        /// </summary>
        IUpdateSourceList Sources { get; }

        /// <summary>
        ///     Period for check new updates.
        /// </summary>
        TimeSpan? CheckUpdatePeriod { get; set; }

        /// <summary>
        ///     Gets the Kontecg configuration object.
        /// </summary>
        IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
