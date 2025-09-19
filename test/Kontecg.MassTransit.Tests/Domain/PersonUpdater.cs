using Castle.Core.Logging;
using Kontecg.Dependency;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.MassTransit.Tests.Domain
{
    public class PersonUpdater : IEventHandler<PersonChangedEventData>, ITransientDependency
    {
        public PersonUpdater()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public void HandleEvent(PersonChangedEventData eventData)
        {
            Logger.Info($"Handling event for {eventData.Entity.Name} on {eventData.EventTime}");
        }
    }
}
