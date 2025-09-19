using System;

namespace Kontecg.MassTransit.Strategies
{
    /// <summary>
    /// Attribute that indicates an event should be published through MassTransit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute provides a declarative way to mark events that should
    /// be distributed through MassTransit in addition to local EventBus.
    /// </para>
    /// <para>
    /// <strong>Usage:</strong>
    /// <code>
    /// [PublishToMassTransit]
    /// public class UserCreatedEventData : IEventData
    /// {
    ///     // event properties
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PublishToDistributedBusAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the PublishToMassTransit attribute.
        /// </summary>
        /// <param name="priority">Message priority (optional)</param>
        /// <param name="exchangeType">Exchange type to use (optional)</param>
        public PublishToDistributedBusAttribute(int priority = 0, string exchangeType = null)
        {
            Priority = priority;
            ExchangeType = exchangeType;
        }

        /// <summary>
        /// Message priority for processing in MassTransit.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Specific exchange type for this event.
        /// </summary>
        public string ExchangeType { get; }
    }
}
