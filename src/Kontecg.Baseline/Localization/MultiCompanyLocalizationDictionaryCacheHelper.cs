using System.Collections.Generic;
using Kontecg.Runtime.Caching;

namespace Kontecg.Localization
{
    /// <summary>
    ///     A helper to implement localization cache.
    /// </summary>
    public static class MultiCompanyLocalizationDictionaryCacheHelper
    {
        /// <summary>
        ///     The cache name.
        /// </summary>
        public const string CacheName = "KontecgBaselineMultiCompanyLocalizationDictionaryCache";

        public static ITypedCache<string, Dictionary<string, string>> GetMultiCompanyLocalizationDictionaryCache(
            this ICacheManager cacheManager)
        {
            return cacheManager.GetCache(CacheName).AsTyped<string, Dictionary<string, string>>();
        }

        public static string CalculateCacheKey(int? companyId, string sourceName, string languageName)
        {
            return sourceName + "#" + languageName + "#" + (companyId ?? 0);
        }
    }
}
