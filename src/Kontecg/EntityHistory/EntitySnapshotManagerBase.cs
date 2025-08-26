using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;
using Kontecg.Json;
using Kontecg.Linq;

namespace Kontecg.EntityHistory
{
    public abstract class EntitySnapshotManagerBase : IEntitySnapshotManager, ITransientDependency
    {
        protected readonly IRepository<EntityChange, long> EntityChangeRepository;

        protected EntitySnapshotManagerBase(IRepository<EntityChange, long> entityChangeRepository)
        {
            EntityChangeRepository = entityChangeRepository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public virtual async Task<EntityHistorySnapshot> GetSnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id,
            DateTime snapshotTime)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            TEntity entity = await GetEntityByIdAsync<TEntity, TPrimaryKey>(id);

            Dictionary<string, string> snapshotPropertiesDictionary = new Dictionary<string, string>();
            Dictionary<string, string> propertyChangesStackTreeDictionary = new Dictionary<string, string>();

            if (entity == null)
            {
                return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
            }

            var changes = await AsyncQueryableExecuter.ToListAsync(
                GetEntityChanges<TEntity, TPrimaryKey>(id, snapshotTime)
                    .Select(x => new {x.ChangeType, x.PropertyChanges})
            );

            //revoke all changes
            foreach (var change in changes) // desc ordered changes
            foreach (EntityPropertyChange entityPropertyChange in change.PropertyChanges)
            {
                RevokeChange<TEntity, TPrimaryKey>(snapshotPropertiesDictionary, entityPropertyChange, entity);

                AddChangeToPropertyChangesStackTree<TEntity, TPrimaryKey>(
                    entityPropertyChange,
                    propertyChangesStackTreeDictionary,
                    entity
                );
            }

            return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
        }

        protected abstract Task<TEntity> GetEntityByIdAsync<TEntity, TPrimaryKey>(TPrimaryKey id)
            where TEntity : class, IEntity<TPrimaryKey>;

        protected abstract IQueryable<EntityChange> GetEntityChanges<TEntity, TPrimaryKey>(TPrimaryKey id,
            DateTime snapshotTime)
            where TEntity : class, IEntity<TPrimaryKey>;

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TPrimaryKey>(
            TPrimaryKey id)
        {
            ParameterExpression lambdaParam = Expression.Parameter(typeof(TEntity));

            MemberExpression leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            object idValue = Convert.ChangeType(id, typeof(TPrimaryKey));

            Expression<Func<object>> closure = () => idValue;
            UnaryExpression rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            BinaryExpression lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        private static void RevokeChange<TEntity, TPrimaryKey>(
            Dictionary<string, string> snapshotPropertiesDictionary,
            EntityPropertyChange entityPropertyChange,
            TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            snapshotPropertiesDictionary[entityPropertyChange.PropertyName] = entityPropertyChange.OriginalValue;
        }

        private static void AddChangeToPropertyChangesStackTree<TEntity, TPrimaryKey>(
            EntityPropertyChange entityPropertyChange,
            Dictionary<string, string> propertyChangesStackTreeDictionary,
            TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (propertyChangesStackTreeDictionary.ContainsKey(entityPropertyChange.PropertyName))
            {
                propertyChangesStackTreeDictionary[entityPropertyChange.PropertyName] =
                    entityPropertyChange.OriginalValue +
                    " -> " +
                    propertyChangesStackTreeDictionary[entityPropertyChange.PropertyName];
            }
            else
            {
                string propertyCurrentValue = "PropertyNotExist";

                PropertyInfo propertyInfo = typeof(TEntity).GetProperty(entityPropertyChange.PropertyName);
                if (propertyInfo != null)
                {
                    object val = propertyInfo.GetValue(entity);
                    propertyCurrentValue = val == null ? "null" : val.ToJsonString();
                }

                propertyChangesStackTreeDictionary.Add(
                    entityPropertyChange.PropertyName,
                    entityPropertyChange.OriginalValue + " -> " + propertyCurrentValue
                );
            }
        }
    }
}
