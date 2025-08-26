using System;

namespace Kontecg.Events.Bus.Entities
{
    [Serializable]
    public class DomainEventEntry
    {
        public DomainEventEntry(object sourceEntity, IEventData eventData)
        {
            SourceEntity = sourceEntity;
            EventData = eventData;
        }

        public object SourceEntity { get; }

        public IEventData EventData { get; }
    }
}
