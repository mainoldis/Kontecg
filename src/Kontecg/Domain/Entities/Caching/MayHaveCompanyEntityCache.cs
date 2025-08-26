using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Caching;

namespace Kontecg.Domain.Entities.Caching
{
    public class MayHaveCompanyEntityCache<TEntity, TCacheItem> :
        MayHaveCompanyEntityCache<TEntity, TCacheItem, int>,
        IMultiCompanyEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>, IMayHaveCompany
    {
        public MayHaveCompanyEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, int> repository,
            string cacheName = null)
            : base(
                cacheManager,
                unitOfWorkManager,
                repository,
                cacheName)
        {
        }
    }

    public class MayHaveCompanyEntityCache<TEntity, TCacheItem, TPrimaryKey> : MultiCompanyEntityCache<TEntity, TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IMayHaveCompany
    {
        public MayHaveCompanyEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, TPrimaryKey> repository,
            string cacheName = null)
            : base(
                cacheManager,
                unitOfWorkManager,
                repository,
                cacheName)
        {
        }

        protected override string GetCacheKey(TEntity entity)
        {
            return GetCacheKey(entity.Id, entity.CompanyId);
        }

        public override string ToString()
        {
            return $"MayHaveCompanyEntityCache {CacheName}";
        }
    }
}
