using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.EntityHistory.Extensions;
using Kontecg.Events.Bus.Entities;
using Kontecg.Extensions;
using Kontecg.Json;
using Kontecg.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kontecg.EntityHistory
{
    /// <summary>
    /// Helper class for managing entity change history.
    /// Provides functionality to track and persist changes to entities, including creation, update, and deletion operations.
    /// </summary>
    public class EntityHistoryHelper : EntityHistoryHelperBase, IEntityHistoryHelper, ITransientDependency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityHistoryHelper"/> class.
        /// </summary>
        /// <param name="configuration">The entity history configuration.</param>
        /// <param name="unitOfWorkManager">The unit of work manager.</param>
        public EntityHistoryHelper(
            IEntityHistoryConfiguration configuration,
            IUnitOfWorkManager unitOfWorkManager)
            : base(configuration, unitOfWorkManager)
        {
        }

        /// <summary>
        /// Creates an entity change set from a collection of entity entries.
        /// </summary>
        /// <param name="entityEntries">The collection of entity entries to process.</param>
        /// <returns>An <see cref="EntityChangeSet"/> containing information about the detected changes.</returns>
        public virtual EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            var changeSet = new EntityChangeSet
            {
                Reason = EntityChangeSetReasonProvider.Reason.TruncateWithPostfix(EntityChangeSet.MaxReasonLength),

                // Fill "who did this change"
                ClientInfo = ClientInfoProvider.ClientInfo.TruncateWithPostfix(EntityChangeSet.MaxClientInfoLength),
                ClientIpAddress =
                    ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityChangeSet.MaxClientIpAddressLength),
                ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(EntityChangeSet.MaxClientNameLength),
                ImpersonatorCompanyId = KontecgSession.ImpersonatorCompanyId,
                ImpersonatorUserId = KontecgSession.ImpersonatorUserId,
                CompanyId = KontecgSession.CompanyId,
                UserId = KontecgSession.UserId
            };

            if (!IsEntityHistoryEnabled)
            {
                return changeSet;
            }

            foreach (var entityEntry in entityEntries)
            {
                var shouldSaveEntityHistory = ShouldSaveEntityHistory(entityEntry);
                if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
                {
                    continue;
                }

                var entityChange = CreateEntityChange(entityEntry);
                if (entityChange == null)
                {
                    continue;
                }

                var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;
                var propertyChanges = GetPropertyChanges(entityEntry, shouldSaveAuditedPropertiesOnly);
                if (propertyChanges.Count == 0)
                {
                    continue;
                }

                entityChange.PropertyChanges = propertyChanges;
                changeSet.EntityChanges.Add(entityChange);
            }

            return changeSet;
        }

        /// <summary>
        /// Asynchronously saves a set of entity changes to the history store.
        /// </summary>
        /// <param name="changeSet">The set of entity changes to be saved.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public virtual async Task SaveAsync(EntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityHistoryStore.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        /// <summary>
        /// Synchronously saves a set of entity changes to the history store.
        /// </summary>
        /// <param name="changeSet">The set of entity changes to be saved.</param>
        public virtual void Save(EntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                EntityHistoryStore.Save(changeSet);
                uow.Complete();
            }
        }

        /// <summary>
        /// Gets the entity identifier from its entry.
        /// </summary>
        /// <param name="entry">The entity entry.</param>
        /// <returns>The entity identifier as a JSON string.</returns>
        protected virtual string GetEntityId(EntityEntry entry)
        {
            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            return primaryKey?.CurrentValue?.ToJsonString() ?? string.Empty;
        }

        /// <summary>
        /// Determines whether the history of an entity should be saved.
        /// </summary>
        /// <param name="entityEntry">The entity entry to evaluate.</param>
        /// <returns>
        /// true if the entity history should be saved,
        /// false if it should not be saved,
        /// null if it should be determined by audited properties.
        /// </returns>
        protected virtual bool? ShouldSaveEntityHistory(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Detached ||
                entityEntry.State == EntityState.Unchanged)
            {
                return false;
            }

            var typeOfEntity = ProxyHelper.GetUnproxiedType(entityEntry.Entity);
            var shouldTrackEntity = IsTypeOfTrackedEntity(typeOfEntity);
            if (shouldTrackEntity.HasValue && !shouldTrackEntity.Value)
            {
                return false;
            }

            if (!IsTypeOfEntity(typeOfEntity) && !entityEntry.Metadata.IsOwned())
            {
                return false;
            }

            var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
            if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
            {
                return false;
            }

            bool? shouldAuditOwnerEntity = null;
            bool? shouldAuditOwnerProperty = null;
            if (!shouldAuditEntity.HasValue && entityEntry.Metadata.IsOwned())
            {
                // Check if owner entity has auditing attribute
                var ownerForeignKey = entityEntry.Metadata.GetForeignKeys().First(fk => fk.IsOwnership);
                var ownerEntityType = ownerForeignKey.PrincipalEntityType.ClrType;

                shouldAuditOwnerEntity = IsTypeOfAuditedEntity(ownerEntityType);
                if (shouldAuditOwnerEntity.HasValue && !shouldAuditOwnerEntity.Value)
                {
                    return false;
                }

                var ownerPropertyInfo = ownerForeignKey.PrincipalToDependent.PropertyInfo;
                shouldAuditOwnerProperty = IsAuditedPropertyInfo(ownerEntityType, ownerPropertyInfo);
                if (shouldAuditOwnerProperty.HasValue && !shouldAuditOwnerProperty.Value)
                {
                    return false;
                }
            }

            return shouldAuditEntity ?? shouldAuditOwnerEntity ?? shouldAuditOwnerProperty ?? shouldTrackEntity;
        }

        /// <summary>
        /// Determines whether the history of a property should be saved.
        /// </summary>
        /// <param name="propertyEntry">The property entry to evaluate.</param>
        /// <param name="defaultValue">The default value if it cannot be determined.</param>
        /// <returns>true if the property history should be saved; otherwise, false.</returns>
        protected virtual bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo == null) // Shadow properties or if mapped directly to a field
            {
                return defaultValue;
            }

            return IsAuditedPropertyInfo(propertyInfo) ?? defaultValue;
        }

        /// <summary>
        /// Creates an entity change entry based on the current state of the entity.
        /// </summary>
        /// <param name="entityEntry">The entity entry to process.</param>
        /// <returns>An <see cref="EntityChange"/> representing the changes made to the entity, or null if there are no changes to record.</returns>
        [CanBeNull]
        private EntityChange CreateEntityChange(EntityEntry entityEntry)
        {
            var entityId = GetEntityId(entityEntry);
            var entityTypeFullName = ProxyHelper.GetUnproxiedType(entityEntry.Entity).FullName;
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                case EntityState.Modified:
                    changeType = entityEntry.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return null;
                default:
                    Logger.ErrorFormat("Unexpected {0} - {1}", nameof(entityEntry.State), entityEntry.State);
                    return null;
            }

            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
                return null;
            }

            return new EntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeFullName = entityTypeFullName,
                CompanyId = KontecgSession.CompanyId
            };
        }

        /// <summary>
        /// Gets the property changes for the specified entry.
        /// </summary>
        /// <param name="entityEntry">The entity entry to inspect.</param>
        /// <param name="auditedPropertiesOnly">Whether to include only audited properties.</param>
        /// <returns>A collection of <see cref="EntityPropertyChange"/> objects representing the property changes.</returns>
        private ICollection<EntityPropertyChange> GetPropertyChanges(EntityEntry entityEntry, bool auditedPropertiesOnly)
        {
            var propertyChanges = new List<EntityPropertyChange>();
            var properties = entityEntry.Metadata.GetProperties();

            foreach (var property in properties)
            {
                if (property.IsPrimaryKey())
                {
                    continue;
                }

                var propertyEntry = entityEntry.Property(property.Name);

                if (ShouldSavePropertyHistory(propertyEntry, !auditedPropertiesOnly))
                {
                    propertyChanges.Add(
                        CreateEntityPropertyChange(
                            propertyEntry.GetOriginalValue(),
                            propertyEntry.GetNewValue(),
                            property
                        )
                    );
                }
            }

            return propertyChanges;
        }

        /// <summary>
        /// Updates change time, entity id, adds foreign keys, and removes or updates property changes after SaveChanges is called.
        /// </summary>
        /// <param name="changeSet">The change set to update.</param>
        private void UpdateChangeSet(EntityChangeSet changeSet)
        {
            var entityChangesToRemove = new List<EntityChange>();
            foreach (var entityChange in changeSet.EntityChanges)
            {
                var entityEntry = entityChange.EntityEntry.As<EntityEntry>();
                var entityEntryType = ProxyHelper.GetUnproxiedType(entityEntry.Entity);
                var isAuditedEntity = IsTypeOfAuditedEntity(entityEntryType) == true;

                /* Update change time */
                entityChange.ChangeTime = GetChangeTime(entityChange.ChangeType, entityEntry.Entity);

                /* Update entity id */
                entityChange.EntityId = GetEntityId(entityEntry);

                /* Update property changes */
                var trackedPropertyNames = entityChange.PropertyChanges.Select(pc => pc.PropertyName).ToList();

                var additionalForeignKeys = entityEntry.Metadata.GetDeclaredReferencingForeignKeys()
                                                    .Where(fk => trackedPropertyNames.Contains(fk.Properties[0].Name))
                                                    .ToList();

                /* Add additional foreign keys from navigation properties */
                foreach (var foreignKey in additionalForeignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var shouldSaveProperty = property.PropertyInfo == null // Shadow properties or if mapped directly to a field
                            ? null
                            : IsAuditedPropertyInfo(entityEntryType, property.PropertyInfo);

                        if (shouldSaveProperty.HasValue && !shouldSaveProperty.Value)
                        {
                            continue;
                        }

                        var propertyEntry = entityEntry.Property(property.Name);

                        var newValue = propertyEntry.GetNewValue();
                        var oldValue = propertyEntry.GetOriginalValue();

                        // Add foreign key
                        entityChange.PropertyChanges.Add(CreateEntityPropertyChange(oldValue, newValue, property));
                    }
                }

                /* Update/Remove property changes */
                var propertyChangesToRemove = new List<EntityPropertyChange>();
                var foreignKeys = entityEntry.Metadata.GetForeignKeys();
                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    var propertyEntry = entityEntry.Property(propertyChange.PropertyName);

                    // Take owner entity type if this is an owned entity
                    var propertyEntityType = entityEntryType;
                    if (entityEntry.Metadata.IsOwned())
                    {
                        var ownerForeignKey = foreignKeys.First(fk => fk.IsOwnership);
                        propertyEntityType = ownerForeignKey.PrincipalEntityType.ClrType;
                    }
                    var property = propertyEntry.Metadata;
                    var isAuditedProperty = property.PropertyInfo != null &&
                                            (IsAuditedPropertyInfo(propertyEntityType, property.PropertyInfo) ?? false);
                    var isForeignKeyShadowProperty = property.IsShadowProperty() && foreignKeys.Any(fk => fk.Properties.Any(p => p.Name == propertyChange.PropertyName));

                    propertyChange.SetNewValue(propertyEntry.GetNewValue()?.ToJsonString());
                    if ((!isAuditedProperty && !isForeignKeyShadowProperty) || propertyChange.IsValuesEquals())
                    {
                        // No change
                        propertyChangesToRemove.Add(propertyChange);
                    }
                }

                foreach (var propertyChange in propertyChangesToRemove)
                {
                    entityChange.PropertyChanges.Remove(propertyChange);
                }

                if (!isAuditedEntity && entityChange.PropertyChanges.Count == 0)
                {
                    entityChangesToRemove.Add(entityChange);
                }
            }

            foreach (var entityChange in entityChangesToRemove)
            {
                changeSet.EntityChanges.Remove(entityChange);
            }
        }

        /// <summary>
        /// Creates an <see cref="EntityPropertyChange"/> object for the specified property.
        /// </summary>
        /// <param name="oldValue">The original value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="property">The property metadata.</param>
        /// <returns>An <see cref="EntityPropertyChange"/> representing the property change.</returns>
        private EntityPropertyChange CreateEntityPropertyChange(object oldValue, object newValue, IProperty property)
        {
            var entityPropertyChange = new EntityPropertyChange()
            {
                PropertyName = property.Name.TruncateWithPostfix(EntityPropertyChange.MaxPropertyNameLength),
                PropertyTypeFullName = property.ClrType.FullName.TruncateWithPostfix(
                    EntityPropertyChange.MaxPropertyTypeFullNameLength
                ),
                CompanyId = KontecgSession.CompanyId
            };

            entityPropertyChange.SetNewValue(newValue?.ToJsonString());
            entityPropertyChange.SetOriginalValue(oldValue?.ToJsonString());
            return entityPropertyChange;
        }
    }
}
