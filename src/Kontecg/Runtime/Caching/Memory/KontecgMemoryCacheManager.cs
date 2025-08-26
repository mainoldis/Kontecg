using Castle.Core.Logging;
using Kontecg.Runtime.Caching.Configuration;

namespace Kontecg.Runtime.Caching.Memory
{
    /// <summary>
    ///     Implements <see cref="ICacheManager" /> to work with MemoryCache.
    /// </summary>
    public class KontecgMemoryCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public KontecgMemoryCacheManager(ICachingConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        protected override ICache CreateCacheImplementation(string name)
        {
            return new KontecgMemoryCache(name, Configuration?.KontecgConfiguration?.Caching?.MemoryCacheOptions)
            {
                Logger = Logger
            };
        }

        protected override void DisposeCaches()
        {
            foreach (ICache cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}
