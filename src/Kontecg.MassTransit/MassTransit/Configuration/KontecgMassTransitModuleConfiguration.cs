using Kontecg.Configuration.Startup;

namespace Kontecg.MassTransit.Configuration
{
    internal class KontecgMassTransitModuleConfiguration : IKontecgMassTransitModuleConfiguration
    {
        public KontecgMassTransitModuleConfiguration(IKontecgStartupConfiguration configuration)
        {
            UseDefaultEventBus = configuration.EventBus.UseDefaultEventBus;
            Options = new RabbitMqOptions();
        }

        /// <inheritdoc />
        public bool UseDefaultEventBus { get; }

        /// <inheritdoc />
        public RabbitMqOptions Options { get; }
    }
}
