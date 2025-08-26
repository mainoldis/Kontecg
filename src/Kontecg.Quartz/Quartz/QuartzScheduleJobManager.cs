using System;
using System.Threading.Tasks;
using Kontecg.BackgroundJobs;
using Kontecg.Dependency;
using Kontecg.Quartz.Configuration;
using Kontecg.Threading.BackgroundWorkers;
using Quartz;

namespace Kontecg.Quartz
{
    public class QuartzScheduleJobManager : BackgroundWorkerBase, IQuartzScheduleJobManager, ISingletonDependency
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IKontecgQuartzConfiguration _quartzConfiguration;

        public QuartzScheduleJobManager(
            IKontecgQuartzConfiguration quartzConfiguration,
            IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            _quartzConfiguration = quartzConfiguration;
            _backgroundJobConfiguration = backgroundJobConfiguration;
        }

        public async Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger)
            where TJob : IJob
        {
            JobBuilder jobToBuild = JobBuilder.Create<TJob>();
            configureJob(jobToBuild);
            IJobDetail job = jobToBuild.Build();

            TriggerBuilder triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            ITrigger trigger = triggerToBuild.Build();

            await _quartzConfiguration.Scheduler.ScheduleJob(job, trigger);
        }

        public async Task RescheduleAsync(TriggerKey triggerKey, Action<TriggerBuilder> configureTrigger)
        {
            TriggerBuilder triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            ITrigger trigger = triggerToBuild.Build();

            await _quartzConfiguration.Scheduler.RescheduleJob(triggerKey, trigger);
        }

        public async Task UnscheduleAsync(TriggerKey triggerKey)
        {
            await _quartzConfiguration.Scheduler.UnscheduleJob(triggerKey);
        }

        public override void Start()
        {
            base.Start();

            if (_backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _ = _quartzConfiguration.Scheduler.Start();
            }

            Logger.Info("Started QuartzScheduleJobManager");
        }

        public override void WaitToStop()
        {
            if (_quartzConfiguration.Scheduler != null)
            {
                try
                {
                    _ = _quartzConfiguration.Scheduler.Shutdown(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            base.WaitToStop();

            Logger.Info("Stopped QuartzScheduleJobManager");
        }
    }
}
