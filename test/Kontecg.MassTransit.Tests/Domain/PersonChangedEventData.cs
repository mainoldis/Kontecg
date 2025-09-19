using Kontecg.Events.Bus.Entities;
using Kontecg.MassTransit.Strategies;

namespace Kontecg.MassTransit.Tests.Domain
{
    [PublishToDistributedBus(exchangeType: "kontecg")]
    public class PersonChangedEventData : EntityChangedEventData<PersonDto>
    {
        /// <inheritdoc />
        public PersonChangedEventData(PersonDto entity) : base(entity)
        {
        }
    }
}
