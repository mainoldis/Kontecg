using System.Collections.Generic;
using Kontecg.Events.Bus;

namespace Kontecg.Domain.Entities
{
    /// <summary>
    /// Defines the contract for aggregate root entities with integer primary keys.
    /// This interface represents the root of an aggregate in Domain-Driven Design (DDD).
    /// </summary>
    /// <remarks>
    /// IAggregateRoot is a convenience interface that simplifies the definition of
    /// aggregate roots that use integer primary keys. It inherits from the generic
    /// IAggregateRoot interface with int as the type parameter, providing a common
    /// pattern for entities that serve as the root of an aggregate. Aggregate roots
    /// are responsible for maintaining the consistency of their aggregate and can
    /// generate domain events when their state changes.
    /// </remarks>
    public interface IAggregateRoot : IAggregateRoot<int>, IEntity
    {

    }

    /// <summary>
    /// Defines the contract for aggregate root entities with configurable primary key types.
    /// This interface represents the root of an aggregate in Domain-Driven Design (DDD).
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the aggregate root entity.</typeparam>
    /// <remarks>
    /// IAggregateRoot is a fundamental interface in Domain-Driven Design that represents
    /// the root of an aggregate. An aggregate is a cluster of domain objects that can
    /// be treated as a single unit for data changes. The aggregate root is responsible
    /// for maintaining the consistency of the aggregate and ensuring that all business
    /// rules are enforced. This interface extends IEntity to provide basic entity
    /// functionality and IGeneratesDomainEvents to support domain event generation,
    /// which is essential for maintaining consistency across aggregates and supporting
    /// eventual consistency patterns.
    /// </remarks>
    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>, IGeneratesDomainEvents
    {

    }

    /// <summary>
    /// Defines the contract for entities that can generate domain events.
    /// This interface provides the ability to collect and manage domain events
    /// that are raised during entity state changes.
    /// </summary>
    /// <remarks>
    /// IGeneratesDomainEvents is used by entities that need to raise domain events
    /// when their state changes. Domain events are a key concept in Domain-Driven Design
    /// that allow for loose coupling between different parts of the domain and support
    /// eventual consistency patterns. Entities that implement this interface can collect
    /// domain events in the DomainEvents collection, which are typically processed
    /// by the domain event infrastructure after the entity is persisted. This pattern
    /// is particularly useful for maintaining consistency across aggregates and for
    /// implementing business processes that span multiple bounded contexts.
    /// </remarks>
    public interface IGeneratesDomainEvents
    {
        /// <summary>
        /// Gets the collection of domain events that have been raised by this entity.
        /// </summary>
        /// <value>
        /// A collection of IEventData objects representing the domain events that
        /// have been raised during the entity's lifecycle.
        /// </value>
        /// <remarks>
        /// This property provides access to the domain events that have been raised
        /// by the entity. Domain events are typically added to this collection when
        /// the entity's state changes in ways that are significant to the business
        /// domain. The events in this collection are usually processed by the domain
        /// event infrastructure after the entity is persisted, allowing for eventual
        /// consistency and loose coupling between different parts of the domain.
        /// The collection should be cleared after the events have been processed to
        /// prevent memory leaks and ensure that events are not processed multiple times.
        /// </remarks>
        ICollection<IEventData> DomainEvents { get; }
    }
}
