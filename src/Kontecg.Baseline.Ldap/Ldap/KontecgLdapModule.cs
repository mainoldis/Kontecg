using System.Reflection;
using Kontecg.Baseline.Ldap.Configuration;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Localization.Sources;
using Kontecg.Modules;

namespace Kontecg.Baseline.Ldap
{
    /// <summary>
    ///     This module extends module chenet to add LDAP authentication.
    /// </summary>
    [DependsOn(typeof(KontecgBaselineModule))]
    public class KontecgLdapModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgLdapModuleConfig, KontecgLdapModuleConfig>();

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    KontecgBaselineConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Kontecg.Baseline.Ldap.Localization.Source")
                )
            );

            Configuration.Settings.Providers.Add<LdapSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
