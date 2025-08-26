using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Extensions;

namespace Kontecg.Baseline.Ldap.Configuration
{
    /// <summary>
    ///     Implements <see cref="ILdapSettings" /> to get settings from <see cref="ISettingManager" />.
    /// </summary>
    public class LdapSettings : ILdapSettings, ITransientDependency
    {
        public LdapSettings(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        protected ISettingManager SettingManager { get; }

        public virtual Task<bool> GetIsEnabledAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync<bool>(LdapSettingNames.IsEnabled, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync<bool>(LdapSettingNames.IsEnabled);
        }

        public virtual async Task<ContextType> GetContextTypeAsync(int? companyId)
        {
            return companyId.HasValue
                ? (await SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.ContextType, companyId.Value))
                .ToEnum<ContextType>()
                : (await SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.ContextType))
                .ToEnum<ContextType>();
        }

        public virtual Task<string> GetContainerAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.Container, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Container);
        }

        public virtual Task<string> GetDomainAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.Domain, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Domain);
        }

        public virtual Task<string> GetUserNameAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.UserName, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.UserName);
        }

        public virtual Task<string> GetPasswordAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync(LdapSettingNames.Password, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Password);
        }

        public Task<bool> GetUseSslAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync<bool>(LdapSettingNames.UseSsl, companyId.Value)
                : SettingManager.GetSettingValueForApplicationAsync<bool>(LdapSettingNames.UseSsl);
        }
    }
}
