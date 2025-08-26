using System.Reflection;
using Kontecg.Dependency;
using Kontecg.Modules;
using Kontecg.Quartz.Configuration;
using Kontecg.Threading;
using Kontecg.Threading.BackgroundWorkers;
using Quartz;

namespace Kontecg.Quartz
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgQuartzModule : KontecgModule
    {
        private static readonly OneTimeRunner OneTimeRunner = new();

        public override void PreInitialize()
        {
            IocManager.Register<IKontecgQuartzConfiguration, KontecgQuartzConfiguration>();

            OneTimeRunner.Run(() =>
            {
                Configuration.Modules.KontecgQuartz().Scheduler.JobFactory =
                    new KontecgQuartzJobFactory(IocManager);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IJobListener, KontecgQuartzJobListener>();

            Configuration.Modules.KontecgQuartz().Scheduler.ListenerManager
                .AddJobListener(IocManager.Resolve<IJobListener>());

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().Add(IocManager.Resolve<IQuartzScheduleJobManager>());
            }
        }
    }
}
