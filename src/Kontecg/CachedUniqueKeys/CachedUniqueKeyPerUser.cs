using System;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.CachedUniqueKeys
{
    public class CachedUniqueKeyPerUser : ICachedUniqueKeyPerUser, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;

        public CachedUniqueKeyPerUser(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public virtual Task<string> GetKeyAsync(string cacheName)
        {
            return GetKeyAsync(cacheName, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        public virtual Task RemoveKeyAsync(string cacheName)
        {
            return RemoveKeyAsync(cacheName, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        public virtual Task<string> GetKeyAsync(string cacheName, UserIdentifier user)
        {
            return GetKeyAsync(cacheName, user.CompanyId, user.UserId);
        }

        public virtual Task RemoveKeyAsync(string cacheName, UserIdentifier user)
        {
            return RemoveKeyAsync(cacheName, user.CompanyId, user.UserId);
        }

        public virtual async Task<string> GetKeyAsync(string cacheName, int? companyId, long? userId)
        {
            if (!KontecgSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            ITypedCache<string, string> cache = GetCache(cacheName);
            return await cache.GetAsync(GetCacheKeyForUser(companyId, userId),
                () => Task.FromResult(Guid.NewGuid().ToString("N")));
        }

        public virtual async Task RemoveKeyAsync(string cacheName, int? companyId, long? userId)
        {
            if (!KontecgSession.UserId.HasValue)
            {
                return;
            }

            ITypedCache<string, string> cache = GetCache(cacheName);
            await cache.RemoveAsync(GetCacheKeyForUser(companyId, userId));
        }

        public virtual async Task ClearCacheAsync(string cacheName)
        {
            ITypedCache<string, string> cache = GetCache(cacheName);
            await cache.ClearAsync();
        }

        public virtual string GetKey(string cacheName)
        {
            return GetKey(cacheName, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        public virtual void RemoveKey(string cacheName)
        {
            RemoveKey(cacheName, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        public virtual string GetKey(string cacheName, UserIdentifier user)
        {
            return GetKey(cacheName, user.CompanyId, user.UserId);
        }

        public virtual void RemoveKey(string cacheName, UserIdentifier user)
        {
            RemoveKey(cacheName, user.CompanyId, user.UserId);
        }

        public virtual string GetKey(string cacheName, int? companyId, long? userId)
        {
            if (!KontecgSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            ITypedCache<string, string> cache = GetCache(cacheName);
            return cache.Get(GetCacheKeyForUser(companyId, userId),
                () => Guid.NewGuid().ToString("N"));
        }

        public virtual void RemoveKey(string cacheName, int? companyId, long? userId)
        {
            if (!KontecgSession.UserId.HasValue)
            {
                return;
            }

            ITypedCache<string, string> cache = GetCache(cacheName);
            cache.Remove(GetCacheKeyForUser(companyId, userId));
        }

        public virtual void ClearCache(string cacheName)
        {
            ITypedCache<string, string> cache = GetCache(cacheName);
            cache.Clear();
        }

        protected virtual ITypedCache<string, string> GetCache(string cacheName)
        {
            return _cacheManager.GetCache<string, string>(cacheName);
        }

        protected virtual string GetCacheKeyForUser(int? companyId, long? userId)
        {
            if (companyId == null)
            {
                return userId.ToString();
            }

            return userId + "@" + companyId;
        }
    }
}
