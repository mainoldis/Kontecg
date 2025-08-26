using System;
using Kontecg.Configuration.Startup;

namespace Kontecg.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public BackgroundJobConfiguration(IKontecgStartupConfiguration kontecgConfiguration)
        {
            KontecgConfiguration = kontecgConfiguration;

            IsJobExecutionEnabled = true;
        }

        public bool IsJobExecutionEnabled { get; set; }

        public TimeSpan? UserTokenExpirationPeriod { get; set; }

        public IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
