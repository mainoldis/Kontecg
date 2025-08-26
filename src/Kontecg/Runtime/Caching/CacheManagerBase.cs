using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kontecg.Dependency;
using Kontecg.Runtime.Caching.Configuration;

namespace Kontecg.Runtime.Caching
{
    /// <summary>
    ///     Base class for cache managers.
    /// </summary>
    public abstract class CacheManagerBase<TCache> : ICacheManager<TCache>, ISingletonDependency
        where TCache : class, ICacheOptions
    {
        protected readonly ConcurrentDictionary<string, TCache> Caches;

        protected readonly ICachingConfiguration Configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        protected CacheManagerBase(ICachingConfiguration configuration)
        {
            Configuration = configuration;
            Caches = new ConcurrentDictionary<string, TCache>();
        }

        public IReadOnlyList<TCache> GetAllCaches()
        {
            return Caches.Values.ToImmutableList();
        }

        public virtual TCache GetCache(string name)
        {
            Check.NotNull(name, nameof(name));

            return Caches.GetOrAdd(name, cacheName =>
            {
                TCache cache = CreateCacheImplementation(cacheName);

                IEnumerable<ICacheConfigurator> configurators =
                    Configuration.Configurators.Where(c => c.CacheName == null || c.CacheName == cacheName);

                foreach (ICacheConfigurator configurator in configurators)
                {
                    configurator.InitAction?.Invoke(cache);
                }

                return cache;
            });
        }

        public virtual void Dispose()
        {
            DisposeCaches();
            Caches.Clear();
        }

        protected abstract void DisposeCaches();

        /// <summary>
        ///     Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected abstract TCache CreateCacheImplementation(string name);
    }
}
