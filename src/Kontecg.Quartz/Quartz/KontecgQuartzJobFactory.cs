using Kontecg.Dependency;
using Kontecg.Extensions;
using Quartz;
using Quartz.Spi;

namespace Kontecg.Quartz
{
    public class KontecgQuartzJobFactory : IJobFactory
    {
        private readonly IIocResolver _iocResolver;

        public KontecgQuartzJobFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            _iocResolver.Release(job);
        }
    }
}
