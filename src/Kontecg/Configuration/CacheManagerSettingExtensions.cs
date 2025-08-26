using System.Collections.Generic;
using Kontecg.Runtime.Caching;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Extension methods for <see cref="ICacheManager" /> to get setting caches.
    /// </summary>
    public static class CacheManagerSettingExtensions
    {
        /// <summary>
        ///     Gets application settings cache.
        /// </summary>
        public static ITypedCache<string, Dictionary<string, SettingInfo>> GetApplicationSettingsCache(
            this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<string, Dictionary<string, SettingInfo>>(KontecgCacheNames.ApplicationSettings);
        }

        /// <summary>
        ///     Gets company settings cache.
        /// </summary>
        public static ITypedCache<int, Dictionary<string, SettingInfo>> GetCompanySettingsCache(
            this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<int, Dictionary<string, SettingInfo>>(KontecgCacheNames.CompanySettings);
        }

        /// <summary>
        ///     Gets user settings cache.
        /// </summary>
        public static ITypedCache<string, Dictionary<string, SettingInfo>> GetUserSettingsCache(
            this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<string, Dictionary<string, SettingInfo>>(KontecgCacheNames.UserSettings);
        }
    }
}
