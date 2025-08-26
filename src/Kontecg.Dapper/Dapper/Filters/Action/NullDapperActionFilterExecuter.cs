using Kontecg.Domain.Entities;

namespace Kontecg.Dapper.Filters.Action
{
    public class NullDapperActionFilterExecuter : IDapperActionFilterExecuter
    {
        public static readonly NullDapperActionFilterExecuter Instance = new();

        public void ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
        }

        public void ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
        }

        public void ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
        }
    }
}
