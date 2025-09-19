using MassTransit;

namespace Kontecg.Events.Bus
{
    public interface IDistributedEventBus
    {
        IBusControl BusControl { get; }
    }
}
