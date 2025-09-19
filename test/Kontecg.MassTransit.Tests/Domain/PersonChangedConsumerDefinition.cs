using MassTransit;
using System;

namespace Kontecg.MassTransit.Tests.Domain
{
    public class PersonChangedConsumerDefinition : ConsumerDefinition<PersonChangedConsumer>
    {
        /// <inheritdoc />
        public PersonChangedConsumerDefinition()
        {
            EndpointName = "sgnom.employee.processing";
            ConcurrentMessageLimit = 4;
        }

        /// <inheritdoc />
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<PersonChangedConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            consumerConfigurator.UseMessageRetry(retry =>
            {
                retry.Interval(3, TimeSpan.FromSeconds(5));
            });
        }
    }
}
