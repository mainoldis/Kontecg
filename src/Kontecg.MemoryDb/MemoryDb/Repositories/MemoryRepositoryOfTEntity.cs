using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;

namespace Kontecg.MemoryDb.Repositories
{
    public class MemoryRepository<TEntity> : MemoryRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public MemoryRepository(IMemoryDatabaseProvider databaseProvider)
            : base(databaseProvider)
        {
        }
    }
}
