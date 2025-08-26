using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.Domain.Entities.Caching
{
    public abstract class MultiCompanyEntityCache<TEntity, TCacheItem, TPrimaryKey> :
        EntityCacheBase<TEntity, TCacheItem, TPrimaryKey>,
        IEventHandler<EntityChangedEventData<TEntity>>,
        IMultiCompanyEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ITypedCache<string, TCacheItem> InternalCache => CacheManager.GetCache<string, TCacheItem>(CacheName);

        public IKontecgSession KontecgSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MultiCompanyEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, TPrimaryKey> repository,
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                unitOfWorkManager,
                cacheName)
        {
            _unitOfWorkManager = unitOfWorkManager;

            KontecgSession = NullKontecgSession.Instance;
        }

        public override TCacheItem Get(TPrimaryKey id)
        {
            return InternalCache.Get(GetCacheKey(id), () => GetCacheItemFromDataSource(id));
        }

        public override Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return InternalCache.GetAsync(GetCacheKey(id), async () => await GetCacheItemFromDataSourceAsync(id));
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(GetCacheKey(eventData.Entity));
        }

        protected virtual int? GetCurrentCompanyId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }

        public virtual string GetCacheKey(TPrimaryKey id)
        {
            return GetCacheKey(id, GetCurrentCompanyId());
        }

        public virtual string GetCacheKey(TPrimaryKey id, int? companyId)
        {
            return id + "@" + (companyId ?? 0);
        }

        protected abstract string GetCacheKey(TEntity entity);

        public override string ToString()
        {
            return $"MultiCompanyEntityCache {CacheName}";
        }
    }
}
