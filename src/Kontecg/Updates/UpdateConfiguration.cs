using System;
using Kontecg.Configuration.Startup;

namespace Kontecg.Updates
{
    internal class UpdateConfiguration : IUpdateConfiguration
    {
        public UpdateConfiguration(IKontecgStartupConfiguration kontecgConfiguration)
        {
            KontecgConfiguration = kontecgConfiguration;
            Sources = new UpdateSourceList();
            IsUpdateCheckEnabled = false;
            IgnoreDeltaUpdates = false;
            CheckForUpdatesAtStartup = true;
        }

        /// <summary>
        ///     Set this flag if applying a release fails to fall back to a full release, which takes longer to download but is
        ///     less error-prone.
        ///     Default: false.
        /// </summary>
        public bool IgnoreDeltaUpdates { get; set; }

        /// <summary>
        ///     Enables/Disables update system.
        ///     Default: false.
        /// </summary>
        public bool IsUpdateCheckEnabled { get; set; }

        /// <summary>
        ///     Check for new updates only once at application start.
        ///     Default: true.
        /// </summary>
        public bool CheckForUpdatesAtStartup { get; set; }

        /// <summary>
        ///     List of update sources.
        /// </summary>
        public IUpdateSourceList Sources { get; }

        /// <summary>
        ///     Period for check new updates.
        /// </summary>
        public TimeSpan? CheckUpdatePeriod { get; set; }

        /// <summary>
        ///     Gets the Kontecg configuration object.
        /// </summary>
        public IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
