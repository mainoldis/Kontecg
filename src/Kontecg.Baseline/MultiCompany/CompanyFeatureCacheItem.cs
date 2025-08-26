using System;
using System.Collections.Generic;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Used to store features of a Company in the cache.
    /// </summary>
    [Serializable]
    public class CompanyFeatureCacheItem
    {
        /// <summary>
        ///     The cache store name.
        /// </summary>
        public const string CacheStoreName = "KontecgBaselineCompanyFeatures";

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyFeatureCacheItem" /> class.
        /// </summary>
        public CompanyFeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Feature values.
        /// </summary>
        public IDictionary<string, string> FeatureValues { get; private set; }
    }
}
