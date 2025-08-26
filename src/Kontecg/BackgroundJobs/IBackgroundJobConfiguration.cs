using System;
using Kontecg.Configuration.Startup;

namespace Kontecg.BackgroundJobs
{
    /// <summary>
    ///     Used to configure background job system.
    /// </summary>
    public interface IBackgroundJobConfiguration
    {
        /// <summary>
        ///     Used to enable/disable background job execution.
        /// </summary>
        bool IsJobExecutionEnabled { get; set; }

        /// <summary>
        ///     Period for user token expiration worker.
        /// </summary>
        TimeSpan? UserTokenExpirationPeriod { get; set; }

        /// <summary>
        ///     Gets the Kontecg configuration object.
        /// </summary>
        IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
