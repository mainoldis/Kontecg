using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using MassTransit;

namespace Kontecg.MassTransit.Tests.Domain
{
    public class PersonChangedConsumer : IConsumer<PersonChangedEventData>, IConsumer<EventMessage<PersonChangedEventData>>, ITransientDependency
    {
        public PersonChangedConsumer()
        {
        }

        /// <inheritdoc />
        public async Task Consume(ConsumeContext<PersonChangedEventData> context)
        {
            var entity = context.Message.Entity;

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task Consume(ConsumeContext<EventMessage<PersonChangedEventData>> context)
        {
            var entity = context.Message.EventData.Entity;

            await Task.CompletedTask;
        }
    }
}
