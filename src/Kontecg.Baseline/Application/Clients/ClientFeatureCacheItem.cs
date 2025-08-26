using System;
using System.Collections.Generic;

namespace Kontecg.Application.Clients
{
    /// <summary>
    ///     Used to store features of a Client in the cache.
    /// </summary>
    [Serializable]
    public class ClientFeatureCacheItem
    {
        /// <summary>
        ///     The cache store name.
        /// </summary>
        public const string CacheStoreName = "KontecgBaselineClientFeatures";

        public IDictionary<string, string> FeatureValues { get; set; }

        public ClientFeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}
