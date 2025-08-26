using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization.Dictionaries;
using Kontecg.Localization.Sources;

namespace Kontecg.Localization
{
    internal class LocalizationManager : ILocalizationManager
    {
        private readonly ILocalizationConfiguration _configuration;
        private readonly IIocResolver _iocResolver;

        private readonly ILanguageManager _languageManager;
        private readonly IDictionary<string, ILocalizationSource> _sources;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public LocalizationManager(
            ILanguageManager languageManager,
            ILocalizationConfiguration configuration,
            IIocResolver iocResolver)
        {
            Logger = NullLogger.Instance;
            _languageManager = languageManager;
            _configuration = configuration;
            _iocResolver = iocResolver;
            _sources = new Dictionary<string, ILocalizationSource>();
        }

        public ILogger Logger { get; set; }

        /// <summary>
        ///     Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        public ILocalizationSource GetSource(string name)
        {
            if (!_configuration.IsEnabled)
            {
                return NullLocalizationSource.Instance;
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!_sources.TryGetValue(name, out ILocalizationSource source))
            {
                throw new KontecgException("Can not find a source with name: " + name);
            }

            return source;
        }

        /// <summary>
        ///     Gets all registered localization sources.
        /// </summary>
        /// <returns>List of sources</returns>
        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _sources.Values.ToImmutableList();
        }

        public void Initialize()
        {
            InitializeSources();
        }

        private void InitializeSources()
        {
            if (!_configuration.IsEnabled)
            {
                Logger.Debug("Localization disabled.");
                return;
            }

            Logger.Debug($"Initializing {_configuration.Sources.Count} localization source" +
                         (_configuration.Sources.Count > 1 ? "s." : "."));
            foreach (ILocalizationSource source in _configuration.Sources)
            {
                if (_sources.ContainsKey(source.Name))
                {
                    throw new KontecgException("There are more than one localization source with name: " + source.Name +
                                               "! Source name must be unique!");
                }

                _sources[source.Name] = source;
                source.Initialize(_configuration, _iocResolver);

                //Extending dictionaries
                if (source is IDictionaryBasedLocalizationSource)
                {
                    IDictionaryBasedLocalizationSource dictionaryBasedSource =
                        source as IDictionaryBasedLocalizationSource;
                    List<LocalizationSourceExtensionInfo> extensions = _configuration.Sources.Extensions
                        .Where(e => e.SourceName == source.Name).ToList();
                    foreach (LocalizationSourceExtensionInfo extension in extensions)
                    {
                        extension.DictionaryProvider.Initialize(source.Name);
                        foreach (ILocalizationDictionary extensionDictionary in extension.DictionaryProvider
                                     .Dictionaries.Values)
                        {
                            dictionaryBasedSource.Extend(extensionDictionary);
                        }
                    }
                }

                Logger.Debug("Initialized localization source: " + source.Name);
            }
        }
    }
}
