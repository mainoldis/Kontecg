using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;

namespace Kontecg.MemoryDb.Repositories
{
    //TODO: Implement thread-safety..?
    public class MemoryRepository<TEntity, TPrimaryKey> : KontecgRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IMemoryDatabaseProvider _databaseProvider;
        private readonly MemoryPrimaryKeyGenerator<TPrimaryKey> _primaryKeyGenerator;

        public MemoryRepository(IMemoryDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _primaryKeyGenerator = new MemoryPrimaryKeyGenerator<TPrimaryKey>();
        }

        public virtual MemoryDatabase Database => _databaseProvider.Database;

        public virtual List<TEntity> Table => Database.Set<TEntity>();

        public override IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public override Task<IQueryable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(Table.AsQueryable());
        }

        public override TEntity Insert(TEntity entity)
        {
            if (entity.IsTransient())
            {
                entity.Id = _primaryKeyGenerator.GetNext();
            }

            Table.Add(entity);
            return entity;
        }

        public override TEntity Update(TEntity entity)
        {
            int index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, entity.Id));
            if (index >= 0)
            {
                Table[index] = entity;
            }

            return entity;
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(TPrimaryKey id)
        {
            int index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, id));
            if (index >= 0)
            {
                Table.RemoveAt(index);
            }
        }
    }
}
