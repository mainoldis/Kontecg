using Kontecg.Configuration.Startup;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;
using Kontecg.Timing;

namespace Kontecg.Dapper.Filters.Action
{
    public class CreationAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        private readonly IMultiCompanyConfig _multiCompanyConfig;

        public CreationAuditDapperActionFilter(IMultiCompanyConfig multiCompanyConfig)
        {
            _multiCompanyConfig = multiCompanyConfig;
        }

        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            long? userId = GetAuditUserId();

            CheckAndSetId(entity);

            IHasCreationTime entityWithCreationTime = entity as IHasCreationTime;
            if (entityWithCreationTime != null)
            {
                if (entityWithCreationTime.CreationTime == default)
                {
                    entityWithCreationTime.CreationTime = Clock.Now;
                }
            }

            CheckAndSetMustHaveCompanyIdProperty(entity);
            CheckAndSetMayHaveCompanyIdProperty(entity);

            if (userId.HasValue && entity is ICreationAudited)
            {
                ICreationAudited record = entity as ICreationAudited;
                if (record.CreatorUserId == null)
                {
                    if (entity is IMayHaveCompany || entity is IMustHaveCompany)
                    {
                        //Sets CreatorUserId only if current user is in same company/host with the given entity
                        if ((entity is IMayHaveCompany &&
                             entity.As<IMayHaveCompany>().CompanyId == KontecgSession.CompanyId) ||
                            (entity is IMustHaveCompany &&
                             entity.As<IMustHaveCompany>().CompanyId == KontecgSession.CompanyId))
                        {
                            record.CreatorUserId = userId;
                        }
                    }
                    else
                    {
                        record.CreatorUserId = userId;
                    }
                }
            }

            if (entity is IHasModificationTime)
            {
                entity.As<IHasModificationTime>().LastModificationTime = null;
            }

            if (entity is IModificationAudited)
            {
                IModificationAudited record = entity.As<IModificationAudited>();
                record.LastModifierUserId = null;
            }
        }

        protected virtual void CheckAndSetMustHaveCompanyIdProperty(object entityAsObj)
        {
            //Only set IMustHaveCompany entities
            if (!(entityAsObj is IMustHaveCompany))
            {
                return;
            }

            IMustHaveCompany entity = entityAsObj.As<IMustHaveCompany>();

            //Don't set if it's already set
            if (entity.CompanyId != 0)
            {
                return;
            }

            int? currentCompanyIdId = GetCurrentCompanyIdOrNull();

            if (currentCompanyIdId != null)
            {
                entity.CompanyId = currentCompanyIdId.Value;
            }
            else
            {
                throw new KontecgException("Can not set CompanyId to 0 for IMustHaveCompany entities!");
            }
        }

        protected virtual void CheckAndSetMayHaveCompanyIdProperty(object entityAsObj)
        {
            //Only set IMayHaveCompany entities
            if (!(entityAsObj is IMayHaveCompany))
            {
                return;
            }

            IMayHaveCompany entity = entityAsObj.As<IMayHaveCompany>();

            //Don't set if it's already set
            if (entity.CompanyId != null)
            {
                return;
            }

            //Only works for single company applications
            if (!_multiCompanyConfig?.IsEnabled ?? false)
            {
                return;
            }

            entity.CompanyId = GetCurrentCompanyIdOrNull();
        }
    }
}
