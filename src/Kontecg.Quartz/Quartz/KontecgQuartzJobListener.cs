using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Quartz;

namespace Kontecg.Quartz
{
    public class KontecgQuartzJobListener : IJobListener
    {
        public KontecgQuartzJobListener()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public string Name { get; } = "KontecgJobListener";

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Logger.Debug($"Job {context.JobDetail.JobType.Name} executing...");
            return Task.FromResult(0);
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Logger.Info($"Job {context.JobDetail.JobType.Name} executing operation vetoed...");
            return Task.FromResult(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = default)
        {
            if (jobException == null)
            {
                Logger.Debug($"Job {context.JobDetail.JobType.Name} successfully executed.");
            }
            else
            {
                Logger.Error($"Job {context.JobDetail.JobType.Name} failed with exception: {jobException}");
            }

            return Task.FromResult(0);
        }
    }
}
