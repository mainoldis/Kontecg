using Kontecg.BackgroundJobs;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;
using Hangfire.Common;
using Hangfire.Server;

namespace Kontecg.Hangfire
{
    public class KontecgHangfireJobExceptionFilter : JobFilterAttribute, IServerFilter, ITransientDependency
    {
        public IEventBus EventBus { get; set; }

        public KontecgHangfireJobExceptionFilter()
        {
            EventBus = NullEventBus.Instance;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                EventBus.Trigger(
                    this,
                    new KontecgHandledExceptionData(
                        new BackgroundJobException(
                            "A background job execution is failed on Hangfire. See inner exception for details. Use JobObject to get Hangfire background job object.",
                            filterContext.Exception
                        )
                        {
                            JobObject = filterContext.BackgroundJob
                        }
                    )
                );
            }
        }
    }
}
