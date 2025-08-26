using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Kontecg.Configuration;
using Kontecg.Localization;

namespace Kontecg.Baseline.Ldap.Configuration
{
    /// <summary>
    ///     Defines LDAP settings.
    /// </summary>
    public class LdapSettingProvider : SettingProvider
    {
        public LdapSettingProvider()
        {
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        protected string LocalizationSourceName { get; set; }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(LdapSettingNames.IsEnabled, "false", L("Ldap_IsEnabled"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false),
                new SettingDefinition(LdapSettingNames.ContextType, ContextType.Domain.ToString(),
                    L("Ldap_ContextType"), scopes: SettingScopes.Application | SettingScopes.Company,
                    isInherited: false),
                new SettingDefinition(LdapSettingNames.Container, null, L("Ldap_Container"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false),
                new SettingDefinition(LdapSettingNames.Domain, null, L("Ldap_Domain"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false),
                new SettingDefinition(LdapSettingNames.UserName, null, L("Ldap_UserName"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false),
                new SettingDefinition(LdapSettingNames.UseSsl, "false", L("Ldap_UseSsl"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false),
                new SettingDefinition(LdapSettingNames.Password, null, L("Ldap_Password"),
                    scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false, isEncrypted: true)
            };
        }

        protected virtual ILocalizableString L(string name)
        {
            return new LocalizableString(name, LocalizationSourceName);
        }
    }
}
