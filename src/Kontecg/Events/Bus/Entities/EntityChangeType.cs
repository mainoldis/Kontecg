namespace Kontecg.Events.Bus.Entities
{
    /// <summary>
    /// Defines the types of changes that can occur to entities in the system.
    /// This enumeration is used to categorize entity change events for auditing,
    /// logging, and event processing purposes.
    /// </summary>
    /// <remarks>
    /// EntityChangeType is used throughout the event bus system to identify the nature
    /// of entity changes. This information is crucial for event handlers that need to
    /// respond differently based on the type of change, as well as for auditing and
    /// compliance requirements. The enumeration uses byte as the underlying type for
    /// efficient storage and serialization, particularly important when dealing with
    /// large volumes of change events.
    /// </remarks>
    public enum EntityChangeType : byte
    {
        /// <summary>
        /// Indicates that a new entity has been created in the system.
        /// </summary>
        /// <remarks>
        /// This value is used when a new entity instance is added to the system.
        /// Created events typically contain the complete entity data and are used
        /// to notify other parts of the system about the new entity's existence.
        /// Event handlers for Created events often perform initialization tasks,
        /// send notifications, or update related data structures.
        /// </remarks>
        Created = 0,

        /// <summary>
        /// Indicates that an existing entity has been modified in the system.
        /// </summary>
        /// <remarks>
        /// This value is used when an existing entity's properties or relationships
        /// have been changed. Updated events may contain the complete entity data
        /// or only the changed properties, depending on the event configuration.
        /// Event handlers for Updated events often perform validation, synchronization,
        /// or notification tasks based on the specific changes made.
        /// </remarks>
        Updated = 1,

        /// <summary>
        /// Indicates that an entity has been removed from the system.
        /// </summary>
        /// <remarks>
        /// This value is used when an entity is deleted or marked as deleted (soft delete).
        /// Deleted events typically contain the entity's identifier and may include
        /// additional metadata about the deletion. Event handlers for Deleted events
        /// often perform cleanup tasks, cascade deletions, or maintain referential
        /// integrity across the system.
        /// </remarks>
        Deleted = 2
    }
}
