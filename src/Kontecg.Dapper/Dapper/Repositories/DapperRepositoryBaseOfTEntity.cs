using Kontecg.Data;
using Kontecg.Domain.Entities;

namespace Kontecg.Dapper.Repositories
{
    public class DapperRepositoryBase<TEntity> : DapperRepositoryBase<TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(
            activeTransactionProvider)
        {
        }
    }
}
