using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Entities;

namespace Kontecg.DynamicEntityProperties.Extensions
{
    public static class DynamicEntityPropertyManagerExtensions
    {
        public static List<DynamicEntityProperty> GetAll<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAll(typeof(TEntity).FullName);
        }

        public static List<DynamicEntityProperty> GetAll<TEntity>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAll<TEntity, int>();
        }

        public static Task<List<DynamicEntityProperty>> GetAllAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAllAsync(typeof(TEntity).FullName);
        }

        public static Task<List<DynamicEntityProperty>> GetAllAsync<TEntity>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAllAsync<TEntity, int>();
        }

        public static DynamicEntityProperty Add<TEntity>(this IDynamicEntityPropertyManager manager,
            int dynamicPropertyId, int? companyId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity, int>(dynamicPropertyId, companyId);
        }

        public static DynamicEntityProperty Add<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager,
            int dynamicPropertyId, int? companyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            DynamicEntityProperty entity = new DynamicEntityProperty
            {
                DynamicPropertyId = dynamicPropertyId,
                EntityFullName = typeof(TEntity).FullName,
                CompanyId = companyId
            };
            manager.Add(entity);
            return entity;
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity>(this IDynamicEntityPropertyManager manager,
            int dynamicPropertyId, int? companyId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity, int>(dynamicPropertyId, companyId);
        }

        public static async Task<DynamicEntityProperty> AddAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyManager manager, int dynamicPropertyId, int? companyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            DynamicEntityProperty entity = new DynamicEntityProperty
            {
                DynamicPropertyId = dynamicPropertyId,
                EntityFullName = typeof(TEntity).FullName,
                CompanyId = companyId
            };
            await manager.AddAsync(entity);
            return entity;
        }

        public static DynamicEntityProperty Add<TEntity>(this IDynamicEntityPropertyManager manager,
            DynamicProperty dynamicProperty, int? companyId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity>(dynamicProperty.Id, companyId);
        }

        public static DynamicEntityProperty Add<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager,
            DynamicProperty dynamicProperty, int? companyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.Add<TEntity, TPrimaryKey>(dynamicProperty.Id, companyId);
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity>(this IDynamicEntityPropertyManager manager,
            DynamicProperty dynamicProperty, int? companyId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity>(dynamicProperty.Id, companyId);
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyManager manager, DynamicProperty dynamicProperty, int? companyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.AddAsync<TEntity, TPrimaryKey>(dynamicProperty.Id, companyId);
        }
    }
}
