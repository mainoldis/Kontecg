using System.Reflection;
using Kontecg.Baseline.ServicioAdmin.Configuration;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Localization.Sources;
using Kontecg.Modules;

namespace Kontecg.Baseline.ServicioAdmin
{
    /// <summary>
    ///     This module extends module baseline to add ServicioAdmin authentication.
    /// </summary>
    [DependsOn(typeof(KontecgBaselineModule))]
    public class KontecgServicioAdminModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgServicioAdminModuleConfig, KontecgServicioAdminModuleConfig>();

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    KontecgBaselineConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Kontecg.Baseline.ServicioAdmin.Localization.Source")
                )
            );

            Configuration.Settings.Providers.Add<ServicioAdminSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
