using System;

namespace Kontecg.Runtime.Caching.Configuration
{
    internal class CacheConfigurator : ICacheConfigurator
    {
        public CacheConfigurator(Action<ICacheOptions> initAction)
        {
            InitAction = initAction;
        }

        public CacheConfigurator(string cacheName, Action<ICacheOptions> initAction)
        {
            CacheName = cacheName;
            InitAction = initAction;
        }

        public string CacheName { get; }

        public Action<ICacheOptions> InitAction { get; }
    }
}
