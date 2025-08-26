using Kontecg.Events.Bus;

namespace Kontecg.Tests.Events.Bus
{
    public abstract class EventBusTestBase
    {
        protected IEventBus EventBus;

        protected EventBusTestBase()
        {
            EventBus = new EventBus();
        }
    }
}