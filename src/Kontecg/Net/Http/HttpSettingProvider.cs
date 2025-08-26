using System.Collections.Generic;
using Kontecg.Configuration;
using Kontecg.Localization;

namespace Kontecg.Net.Http
{
    /// <summary>
    ///     Defines settings to send http requests.
    ///     <see cref="HttpSettingNames" /> for all available configurations.
    /// </summary>
    internal class HttpSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(HttpSettingNames.Proxy.Uri, "", L("ProxyUri"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.Proxy.UserName, "", L("Username"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.Proxy.Password, "", L("Password"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.Proxy.Domain, "", L("DomainName"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.Proxy.BypassOnLocal, "false", L("BypassOnLocal"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.Proxy.BypassList, "", L("BypassList"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.XApplicationName, "KONTECG", L("DefaultRequestHeader"),
                    scopes: SettingScopes.Application | SettingScopes.Company),
                new SettingDefinition(HttpSettingNames.UseDefaultProxy, "true", L("UseDefaultProxy"),
                    scopes: SettingScopes.Application | SettingScopes.Company)
            };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgConsts.LocalizationSourceName);
        }
    }
}
