using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Uow;
using Kontecg.Reflection;
using Kontecg.Runtime.Session;

namespace Kontecg.Dapper.Filters.Action
{
    public abstract class DapperActionFilterBase
    {
        protected DapperActionFilterBase()
        {
            KontecgSession = NullKontecgSession.Instance;
            GuidGenerator = UuidGenerator.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        public IGuidGenerator GuidGenerator { get; set; }

        protected virtual long? GetAuditUserId()
        {
            if (KontecgSession.UserId.HasValue && CurrentUnitOfWorkProvider?.Current != null)
            {
                return KontecgSession.UserId;
            }

            return null;
        }

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            IEntity<Guid> entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                Type entityType = entityAsObj.GetType();
                PropertyInfo idProperty = entityType.GetProperty("Id");
                DatabaseGeneratedAttribute dbGeneratedAttr =
                    ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual int? GetCurrentCompanyIdOrNull()
        {
            if (CurrentUnitOfWorkProvider?.Current != null)
            {
                return CurrentUnitOfWorkProvider.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }
    }
}
