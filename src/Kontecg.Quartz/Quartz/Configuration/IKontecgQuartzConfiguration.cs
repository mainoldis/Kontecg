using Quartz;

namespace Kontecg.Quartz.Configuration
{
    public interface IKontecgQuartzConfiguration
    {
        IScheduler Scheduler { get; }
    }
}
