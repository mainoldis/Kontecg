using Kontecg.Application.Clients;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.MultiCompany;

namespace Kontecg.Runtime.Caching
{
    public static class KontecgBaselineCacheManagerExtensions
    {
        public static ITypedCache<string, UserPermissionCacheItem> GetUserPermissionCache(
            this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, UserPermissionCacheItem>(UserPermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, RolePermissionCacheItem> GetRolePermissionCache(
            this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, RolePermissionCacheItem>(RolePermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<int, CompanyFeatureCacheItem> GetCompanyFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, CompanyFeatureCacheItem>(CompanyFeatureCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, ClientFeatureCacheItem> GetClientFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, ClientFeatureCacheItem>(ClientFeatureCacheItem.CacheStoreName);
        }
    }
}
