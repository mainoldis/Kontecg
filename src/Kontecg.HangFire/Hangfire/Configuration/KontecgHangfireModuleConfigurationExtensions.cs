using System;
using Kontecg.BackgroundJobs;
using Kontecg.Configuration.Startup;

namespace Kontecg.Hangfire.Configuration
{
    public static class KontecgHangfireConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Kontecg Hangfire module.
        /// </summary>
        public static IKontecgHangfireConfiguration UseHangfire(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgHangfireConfiguration>();
        }

        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration, Action<IKontecgHangfireConfiguration> configureAction)
        {
            backgroundJobConfiguration.KontecgConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
            configureAction(backgroundJobConfiguration.KontecgConfiguration.Modules.UseHangfire());
        }
    }
}
