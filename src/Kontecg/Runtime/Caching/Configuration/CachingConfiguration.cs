using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kontecg.Configuration.Startup;
using Microsoft.Extensions.Caching.Memory;

namespace Kontecg.Runtime.Caching.Configuration
{
    internal class CachingConfiguration : ICachingConfiguration
    {
        private readonly List<ICacheConfigurator> _configurators;

        public CachingConfiguration(IKontecgStartupConfiguration kontecgConfiguration)
        {
            KontecgConfiguration = kontecgConfiguration;

            _configurators = new List<ICacheConfigurator>();
        }

        public IKontecgStartupConfiguration KontecgConfiguration { get; }

        public IReadOnlyList<ICacheConfigurator> Configurators => _configurators.ToImmutableList();

        public MemoryCacheOptions MemoryCacheOptions { get; set; }

        public void ConfigureAll(Action<ICacheOptions> initAction)
        {
            _configurators.Add(new CacheConfigurator(initAction));
        }

        public void Configure(string cacheName, Action<ICacheOptions> initAction)
        {
            _configurators.Add(new CacheConfigurator(cacheName, initAction));
        }
    }
}
