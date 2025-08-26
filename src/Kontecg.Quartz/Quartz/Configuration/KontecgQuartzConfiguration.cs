using Quartz;
using Quartz.Impl;

namespace Kontecg.Quartz.Configuration
{
    public class KontecgQuartzConfiguration : IKontecgQuartzConfiguration
    {
        public IScheduler Scheduler =>
            StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
