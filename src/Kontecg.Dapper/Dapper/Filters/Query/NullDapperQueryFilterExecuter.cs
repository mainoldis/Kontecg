using System;
using System.Linq.Expressions;
using DapperExtensions;
using Kontecg.Domain.Entities;

namespace Kontecg.Dapper.Filters.Query
{
    public class NullDapperQueryFilterExecuter : IDapperQueryFilterExecuter
    {
        public static readonly NullDapperQueryFilterExecuter Instance = new();

        public IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return null;
        }

        public PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return null;
        }
    }
}
