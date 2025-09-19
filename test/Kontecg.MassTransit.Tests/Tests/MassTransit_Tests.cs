using System;
using System.Threading.Tasks;
using Kontecg.Events.Bus;
using Kontecg.Json;
using Kontecg.MassTransit.Tests.Domain;
using Xunit;

namespace Kontecg.MassTransit.Tests.Tests
{
    public class MassTransit_Tests : KontecgMassTransitTestBase
    {
        [Fact]
        public void Should_register_MassTransitBus_Test()
        {
            var eventBus = Resolve<IEventBus>();
            var person = new PersonDto(1, "Mainoldis Fuentes Suárez");

            eventBus.Trigger(new PersonChangedEventData(person));
        }

        [Fact]
        public async Task Should_probe_some_messages_Test()
        {
        }
    }
}
