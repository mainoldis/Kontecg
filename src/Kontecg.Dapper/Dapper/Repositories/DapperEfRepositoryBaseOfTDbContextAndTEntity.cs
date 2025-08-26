using Kontecg.Data;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Uow;

namespace Kontecg.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity> : DapperEfRepositoryBase<TDbContext, TEntity, int>,
        IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : class

    {
        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(activeTransactionProvider, currentUnitOfWorkProvider)
        {
        }
    }
}
