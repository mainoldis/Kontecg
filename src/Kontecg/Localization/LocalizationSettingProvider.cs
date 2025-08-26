using System.Collections.Generic;
using Kontecg.Configuration;

namespace Kontecg.Localization
{
    public class LocalizationSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(LocalizationSettingNames.DefaultLanguage, null, L("DefaultLanguage"),
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
