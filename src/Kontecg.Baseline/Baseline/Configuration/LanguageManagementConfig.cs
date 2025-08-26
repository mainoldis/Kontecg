using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Baseline.Configuration
{
    internal class LanguageManagementConfig : ILanguageManagementConfig
    {
        private readonly IKontecgStartupConfiguration _configuration;

        private readonly IIocManager _iocManager;

        public LanguageManagementConfig(IIocManager iocManager, IKontecgStartupConfiguration configuration)
        {
            _iocManager = iocManager;
            _configuration = configuration;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void EnableDbLocalization()
        {
            _iocManager.Register<ILanguageProvider, ApplicationLanguageProvider>(DependencyLifeStyle.Transient);

            List<IDictionaryBasedLocalizationSource> sources = _configuration
                .Localization
                .Sources
                .Where(s => s is IDictionaryBasedLocalizationSource)
                .Cast<IDictionaryBasedLocalizationSource>()
                .ToList();

            foreach (IDictionaryBasedLocalizationSource source in sources)
            {
                _configuration.Localization.Sources.Remove(source);
                _configuration.Localization.Sources.Add(
                    new MultiCompanyLocalizationSource(
                        source.Name,
                        new MultiCompanyLocalizationDictionaryProvider(
                            source.DictionaryProvider,
                            _iocManager
                        )
                    )
                );

                Logger.DebugFormat("Converted {0} ({1}) to MultiCompanyLocalizationSource", source.Name,
                    source.GetType());
            }
        }
    }
}
