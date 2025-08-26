using Kontecg.Domain.Entities;

namespace Kontecg.Dapper.Repositories
{
    public interface IDapperRepository<TEntity> : IDapperRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }
}
