namespace Kontecg.MassTransit.Configuration
{
    public interface IKontecgMassTransitModuleConfiguration
    {
        bool UseDefaultEventBus { get; }

        RabbitMqOptions Options { get; }
    }
}
