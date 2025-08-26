using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Kontecg.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Runtime.Session;
using Kontecg.Timing;

namespace Kontecg.EntityHistory
{
    public abstract class EntityHistoryHelperBase
    {
        protected readonly IEntityHistoryConfiguration EntityHistoryConfiguration;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        protected EntityHistoryHelperBase(
            IEntityHistoryConfiguration entityHistoryConfiguration,
            IUnitOfWorkManager unitOfWorkManager)
        {
            EntityHistoryConfiguration = entityHistoryConfiguration;
            UnitOfWorkManager = unitOfWorkManager;

            KontecgSession = NullKontecgSession.Instance;
            Logger = NullLogger.Instance;
            ClientInfoProvider = NullClientInfoProvider.Instance;
            EntityChangeSetReasonProvider = NullEntityChangeSetReasonProvider.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }

        public ILogger Logger { get; set; }
        public IKontecgSession KontecgSession { get; set; }
        public IClientInfoProvider ClientInfoProvider { get; set; }
        public IEntityChangeSetReasonProvider EntityChangeSetReasonProvider { get; set; }
        public IEntityHistoryStore EntityHistoryStore { get; set; }

        protected bool IsEntityHistoryEnabled => EntityHistoryConfiguration.IsEnabled &&
                                                 (KontecgSession.UserId.HasValue || EntityHistoryConfiguration
                                                     .IsEnabledForAnonymousUsers);

        protected virtual DateTime GetChangeTime(EntityChangeType entityChangeType, object entity)
        {
            switch (entityChangeType)
            {
                case EntityChangeType.Created:
                    return (entity as IHasCreationTime)?.CreationTime ?? Clock.Now;
                case EntityChangeType.Deleted:
                    return (entity as IHasDeletionTime)?.DeletionTime ?? Clock.Now;
                case EntityChangeType.Updated:
                    return (entity as IHasModificationTime)?.LastModificationTime ?? Clock.Now;
                default:
                    Logger.ErrorFormat("Unexpected {0} - {1}", nameof(entityChangeType), entityChangeType);
                    return Clock.Now;
            }
        }

        protected virtual bool IsTypeOfEntity(Type entityType)
        {
            return EntityHelper.IsEntity(entityType) && entityType.IsPublic;
        }

        protected virtual bool? IsTypeOfAuditedEntity(Type entityType)
        {
            TypeInfo entityTypeInfo = entityType.GetTypeInfo();
            if (entityTypeInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (entityTypeInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }

        protected virtual bool? IsTypeOfTrackedEntity(Type entityType)
        {
            if (EntityHistoryConfiguration.IgnoredTypes.Any(type => type.GetTypeInfo().IsAssignableFrom(entityType)))
            {
                return false;
            }

            if (EntityHistoryConfiguration.Selectors.Any(selector => selector.Predicate(entityType)))
            {
                return true;
            }

            return null;
        }

        protected virtual bool? IsAuditedPropertyInfo(Type entityType, PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            bool? isTrackedEntity = IsTypeOfTrackedEntity(entityType);
            bool? isAuditedEntity = IsTypeOfAuditedEntity(entityType);

            return (isTrackedEntity ?? false) || (isAuditedEntity ?? false);
        }

        protected virtual bool? IsAuditedPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }
    }
}
