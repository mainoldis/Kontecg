using Kontecg.Dependency;
using Kontecg.Domain.Entities;

namespace Kontecg.Dapper.Filters.Action
{
    public interface IDapperActionFilter : ITransientDependency
    {
        void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
