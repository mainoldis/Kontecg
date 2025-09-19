using System;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.MassTransit.Abstractions;

namespace Kontecg.MassTransit.Mappers
{
    public class DirectEventMessageMapper : IEventMessageMapper, ITransientDependency
    {
        /// <inheritdoc />
        public object MapToMessage<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return eventData ?? throw new ArgumentNullException(nameof(eventData));
        }
    }
}
