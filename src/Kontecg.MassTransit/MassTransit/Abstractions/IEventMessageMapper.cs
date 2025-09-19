using Kontecg.Events.Bus;
using Kontecg.MassTransit.Mappers;

namespace Kontecg.MassTransit.Abstractions
{
    /// <summary>
    /// Interface for mapping local events to MassTransit messages.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface allows transforming local domain events into messages optimized
    /// for distribution through MassTransit, including serialization, versioning, and metadata.
    /// </para>
    /// </remarks>
    public interface IEventMessageMapper
    {
        /// <summary>
        /// Maps a local event to a message for MassTransit.
        /// </summary>
        /// <typeparam name="TEventData">Type of local event</typeparam>
        /// <param name="eventData">Event to map</param>
        /// <returns>Object that will be published through MassTransit</returns>
        /// <remarks>
        /// <para>
        /// Allows transformations such as:
        /// <list type="bullet">
        /// <item><description>Adding routing metadata</description></item>
        /// <item><description>Applying message versioning</description></item>
        /// <item><description>Filtering sensitive properties</description></item>
        /// <item><description>Enriching with additional context</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        object MapToMessage<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}
