using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Dependency;
using Newtonsoft.Json.Linq;
using Velopack.Sources;

namespace Kontecg.Updates
{
    /// <summary>
    ///     This interface is used to manage updates system.
    /// </summary>
    internal class KontecgUpdateManager : IUpdateManager
    {
        private readonly IUpdateConfiguration _configuration;
        private readonly IIocResolver _iocResolver;
        private readonly IDictionary<string, IUpdateSource> _sources;

        public KontecgUpdateManager(
            IUpdateConfiguration configuration,
            IIocResolver iocResolver,
            IFileDownloader fileDownloader)
        {
            Logger = NullLogger.Instance;
            _configuration = configuration;
            _iocResolver = iocResolver;
            _sources = new Dictionary<string, IUpdateSource>();
        }

        public ILogger Logger { get; set; }

        public async Task<bool> CheckForUpdateAsync(Action<int> progress = null)
        {
            progress = progress ?? (i => { });

            if (_iocResolver.IsRegistered<IUpdateChecker>())
            {
                using IDisposableDependencyObjectWrapper<IUpdateChecker> checker =
                    _iocResolver.ResolveAsDisposable<IUpdateChecker>();
                JObject result = await checker.Object.CheckAsync(_sources.Values, progress);
            }

            progress?.Invoke(100);
            return false;
        }

        public void Initialize()
        {
            InitializeSources();
        }

        private void InitializeSources()
        {
            if (!_configuration.IsUpdateCheckEnabled)
            {
                Logger.Debug("Updates are disabled.");
                return;
            }

            Logger.Debug($"Initializing {_configuration.Sources.Count} update source" +
                         (_configuration.Sources.Count > 1 ? "s." : "."));
            foreach (IUpdateSource source in _configuration.Sources)
            {
                if (_sources.ContainsKey(source.Name))
                {
                    throw new KontecgException("There are more than one update source with name: " + source.Name +
                                               "! Source name must be unique!");
                }

                _sources[source.Name] = source;
                source.Initialize(_configuration, _iocResolver);
                Logger.Debug("Initialized update source: " + source.Name);
            }
        }
    }
}
