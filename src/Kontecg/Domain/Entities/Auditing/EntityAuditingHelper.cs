using System.Collections.Generic;
using System.Linq;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Timing;

namespace Kontecg.Domain.Entities.Auditing
{
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(
            IMultiCompanyConfig multiCompanyConfig,
            object entityAsObj,
            int? companyId,
            long? userId,
            IReadOnlyList<AuditFieldConfiguration> auditFields)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default)
            {
                entityWithCreationTime.CreationTime = Clock.Now;
            }

            if (!(entityAsObj is ICreationAudited))
            {
                //Object does not implement ICreationAudited
                return;
            }

            if (!userId.HasValue)
            {
                //Unknown user
                return;
            }

            var entity = entityAsObj as ICreationAudited;
            if (entity.CreatorUserId != null)
            {
                //CreatorUserId is already set
                return;
            }

            if (multiCompanyConfig?.IsEnabled == true)
            {
                if (MultiCompanyHelper.IsMultiCompanyEntity(entity) &&
                    !MultiCompanyHelper.IsCompanyEntity(entity, companyId))
                {
                    //A company entity is created by host or a different company
                    return;
                }

                if (companyId.HasValue && MultiCompanyHelper.IsHostEntity(entity))
                {
                    //Company user created a host entity
                    return;
                }
            }

            var creationUserIdFilter = auditFields?.FirstOrDefault(e => e.FieldName == KontecgAuditFields.CreatorUserId);
            if (creationUserIdFilter != null && !creationUserIdFilter.IsSavingEnabled)
            {
                return;
            }

            //Finally, set CreatorUserId!
            entity.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(
            IMultiCompanyConfig multiCompanyConfig,
            object entityAsObj,
            int? companyId,
            long? userId,
            IReadOnlyList<AuditFieldConfiguration> auditFields)
        {
            if (entityAsObj is IHasModificationTime)
            {
                var lastModificationTimeFilter = auditFields?.FirstOrDefault(e => e.FieldName == KontecgAuditFields.LastModificationTime);
                if (lastModificationTimeFilter == null || lastModificationTimeFilter.IsSavingEnabled)
                {
                    entityAsObj.As<IHasModificationTime>().LastModificationTime = Clock.Now;
                }
            }

            if (!(entityAsObj is IModificationAudited))
            {
                //Entity does not implement IModificationAudited
                return;
            }

            var entity = entityAsObj.As<IModificationAudited>();

            if (userId == null)
            {
                //Unknown user
                entity.LastModifierUserId = null;
                return;
            }

            if (multiCompanyConfig?.IsEnabled == true)
            {
                if (MultiCompanyHelper.IsMultiCompanyEntity(entity) &&
                    !MultiCompanyHelper.IsCompanyEntity(entity, companyId))
                {
                    //A company entity is modified by host or a different company
                    entity.LastModifierUserId = null;
                    return;
                }

                if (companyId.HasValue && MultiCompanyHelper.IsHostEntity(entity))
                {
                    //Company user modified a host entity
                    entity.LastModifierUserId = null;
                    return;
                }
            }

            var lastModifierUserIdFilter = auditFields?.FirstOrDefault(e => e.FieldName == KontecgAuditFields.LastModifierUserId);
            if (lastModifierUserIdFilter != null && !lastModifierUserIdFilter.IsSavingEnabled)
            {
                return;
            }

            //Finally, set LastModifierUserId!
            entity.LastModifierUserId = userId;
        }

        public static void SetDeletionAuditProperties(
            IMultiCompanyConfig multiCompanyConfig,
            object entityAsObj,
            int? companyId,
            long? userId,
            IReadOnlyList<AuditFieldConfiguration> auditFields)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                if (entity.DeletionTime == null)
                {
                    var deletionTimeFilter = auditFields?.FirstOrDefault(e => e.FieldName == KontecgAuditFields.DeletionTime);
                    if (deletionTimeFilter == null || deletionTimeFilter.IsSavingEnabled)
                    {
                        entityAsObj.As<IHasDeletionTime>().DeletionTime = Clock.Now;
                    }
                }
            }

            if (entityAsObj is IDeletionAudited)
            {
                var entity = entityAsObj.As<IDeletionAudited>();

                if (entity.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    entity.DeleterUserId = null;
                    return;
                }

                var deleterUserIdFilter = auditFields?.FirstOrDefault(e => e.FieldName == KontecgAuditFields.DeleterUserId);
                if (deleterUserIdFilter != null && !deleterUserIdFilter.IsSavingEnabled)
                {
                    return;
                }

                //Special check for multi-company entities
                if (entity is IMayHaveCompany || entity is IMustHaveCompany)
                {
                    //Sets LastModifierUserId only if current user is in same company/host with the given entity
                    if ((entity is IMayHaveCompany && entity.As<IMayHaveCompany>().CompanyId == companyId) ||
                        (entity is IMustHaveCompany && entity.As<IMustHaveCompany>().CompanyId == companyId))
                    {
                        entity.DeleterUserId = userId;
                    }
                    else
                    {
                        entity.DeleterUserId = null;
                    }
                }
                else
                {
                    entity.DeleterUserId = userId;
                }
            }
        }
    }
}
