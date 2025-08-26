using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Events.Bus;

namespace Kontecg.BackgroundJobs
{
    public class EventTriggerAsyncBackgroundJob<TEventData> : IAsyncBackgroundJob<TEventData>, ITransientDependency
        where TEventData : EventData
    {
        public EventTriggerAsyncBackgroundJob()
        {
            EventBus = NullEventBus.Instance;
        }

        public IEventBus EventBus { get; set; }

        public async Task ExecuteAsync(TEventData eventData)
        {
            await EventBus.TriggerAsync(eventData);
        }
    }
}
