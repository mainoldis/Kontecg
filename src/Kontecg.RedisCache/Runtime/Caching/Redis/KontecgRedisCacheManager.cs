using Kontecg.Dependency;
using Kontecg.Runtime.Caching.Configuration;

namespace Kontecg.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="KontecgRedisCache"/> instances.
    /// </summary>
    public class KontecgRedisCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="KontecgRedisCacheManager"/> class.
        /// </summary>
        public KontecgRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<KontecgRedisCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<KontecgRedisCache>(new { name });
        }
        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }
    }
}
