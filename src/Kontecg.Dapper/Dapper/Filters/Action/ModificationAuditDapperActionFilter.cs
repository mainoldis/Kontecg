using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;
using Kontecg.Timing;

namespace Kontecg.Dapper.Filters.Action
{
    public class ModificationAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity is IHasModificationTime)
            {
                entity.As<IHasModificationTime>().LastModificationTime = Clock.Now;
            }

            if (entity is IModificationAudited)
            {
                IModificationAudited record = entity.As<IModificationAudited>();
                long? userId = GetAuditUserId();
                if (userId == null)
                {
                    record.LastModifierUserId = null;
                    return;
                }

                //Special check for multi-company entities
                if (entity is IMayHaveCompany || entity is IMustHaveCompany)
                {
                    //Sets LastModifierUserId only if current user is in same company/host with the given entity
                    if ((entity is IMayHaveCompany &&
                         entity.As<IMayHaveCompany>().CompanyId == KontecgSession.CompanyId) ||
                        (entity is IMustHaveCompany &&
                         entity.As<IMustHaveCompany>().CompanyId == KontecgSession.CompanyId))
                    {
                        record.LastModifierUserId = userId;
                    }
                    else
                    {
                        record.LastModifierUserId = null;
                    }
                }
                else
                {
                    record.LastModifierUserId = userId;
                }
            }
        }
    }
}
