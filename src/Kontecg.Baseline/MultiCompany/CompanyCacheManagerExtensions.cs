using Kontecg.Runtime.Caching;

namespace Kontecg.MultiCompany
{
    public static class CompanyCacheManagerExtensions
    {
        public static ITypedCache<int, CompanyCacheItem> GetCompanyCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, CompanyCacheItem>(CompanyCacheItem.CacheName);
        }

        public static ITypedCache<string, int?> GetCompanyByNameCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, int?>(CompanyCacheItem.ByNameCacheName);
        }
    }
}
