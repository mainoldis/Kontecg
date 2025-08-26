using System.Collections.Generic;
using Kontecg.Configuration;
using Kontecg.Localization;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    /// <summary>
    ///     Defines ServicioAdmin settings.
    /// </summary>
    public class ServicioAdminSettingProvider : SettingProvider
    {
        public ServicioAdminSettingProvider()
        {
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        protected string LocalizationSourceName { get; set; }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(ServicioAdminSettingNames.IsEnabled, "true", L("ServicioAdmin_IsEnabled"),
                    scopes: SettingScopes.Company, isInherited: false)
            };
        }

        protected virtual ILocalizableString L(string name)
        {
            return new LocalizableString(name, LocalizationSourceName);
        }
    }
}
