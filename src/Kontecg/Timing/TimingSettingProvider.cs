using System.Collections.Generic;
using Kontecg.Configuration;
using Kontecg.Localization;

namespace Kontecg.Timing
{
    public class TimingSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(TimingSettingNames.TimeZone, "UTC", L("TimeZone"),
                    scopes: SettingScopes.Application | SettingScopes.Company | SettingScopes.User,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider())
            };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgConsts.LocalizationSourceName);
        }
    }
}
