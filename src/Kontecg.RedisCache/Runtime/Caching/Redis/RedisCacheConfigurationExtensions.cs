using System;
using Kontecg.Dependency;
using Kontecg.RealTime;
using Kontecg.Runtime.Caching.Configuration;
using Kontecg.Runtime.Caching.Redis.RealTime;

namespace Kontecg.Runtime.Caching.Redis
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseRedis(options => { });
        }

        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<KontecgRedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.KontecgConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, KontecgRedisCacheManager>();
            iocManager.RegisterIfNot<IOnlineClientStore, RedisOnlineClientStore>();
            
            optionsAction(iocManager.Resolve<KontecgRedisCacheOptions>());
        }
    }
}
