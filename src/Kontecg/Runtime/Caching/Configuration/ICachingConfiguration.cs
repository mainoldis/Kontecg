using System;
using System.Collections.Generic;
using Kontecg.Configuration.Startup;
using Microsoft.Extensions.Caching.Memory;

namespace Kontecg.Runtime.Caching.Configuration
{
    /// <summary>
    ///     Used to configure caching system.
    /// </summary>
    public interface ICachingConfiguration
    {
        /// <summary>
        ///     Gets the Kontecg configuration object.
        /// </summary>
        IKontecgStartupConfiguration KontecgConfiguration { get; }

        IReadOnlyList<ICacheConfigurator> Configurators { get; }

        /// <summary>
        /// Options for memory cache
        /// </summary>
        MemoryCacheOptions MemoryCacheOptions { get; set; }

        /// <summary>
        /// Used to configure all caches.
        /// </summary>
        /// <param name="initAction">
        /// An action to configure caches
        /// This action is called for each cache just after created.
        /// </param>
        void ConfigureAll(Action<ICacheOptions> initAction);

        /// <summary>
        /// Used to configure a specific cache. 
        /// </summary>
        /// <param name="cacheName">Cache name</param>
        /// <param name="initAction">
        /// An action to configure the cache.
        /// This action is called just after the cache is created.
        /// </param>
        void Configure(string cacheName, Action<ICacheOptions> initAction);
    }
}
