using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;
using Kontecg.Timing;

namespace Kontecg.Dapper.Filters.Action
{
    public class DeletionAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity is ISoftDelete)
            {
                ISoftDelete record = entity.As<ISoftDelete>();
                record.IsDeleted = true;
            }

            if (entity is IHasDeletionTime)
            {
                IHasDeletionTime record = entity.As<IHasDeletionTime>();
                if (record.DeletionTime == null)
                {
                    record.DeletionTime = Clock.Now;
                }
            }

            if (entity is IDeletionAudited)
            {
                long? userId = GetAuditUserId();
                IDeletionAudited record = entity.As<IDeletionAudited>();

                if (record.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    record.DeleterUserId = null;
                    return;
                }

                if (entity is IMayHaveCompany || entity is IMustHaveCompany)
                {
                    //Sets LastModifierUserId only if current user is in same company/host with the given entity
                    if ((entity is IMayHaveCompany &&
                         entity.As<IMayHaveCompany>().CompanyId == KontecgSession.CompanyId) ||
                        (entity is IMustHaveCompany &&
                         entity.As<IMustHaveCompany>().CompanyId == KontecgSession.CompanyId))
                    {
                        record.DeleterUserId = userId;
                    }
                    else
                    {
                        record.DeleterUserId = null;
                    }
                }
                else
                {
                    record.DeleterUserId = userId;
                }
            }
        }
    }
}
