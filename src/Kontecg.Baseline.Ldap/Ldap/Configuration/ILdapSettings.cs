using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace Kontecg.Baseline.Ldap.Configuration
{
    /// <summary>
    ///     Used to obtain current values of LDAP settings.
    ///     This abstraction allows to define a different source for settings than SettingManager (see default implementation:
    ///     <see cref="LdapSettings" />).
    /// </summary>
    public interface ILdapSettings
    {
        Task<bool> GetIsEnabledAsync(int? companyId);

        Task<ContextType> GetContextTypeAsync(int? companyId);

        Task<string> GetContainerAsync(int? companyId);

        Task<string> GetDomainAsync(int? companyId);

        Task<string> GetUserNameAsync(int? companyId);

        Task<string> GetPasswordAsync(int? companyId);

        Task<bool> GetUseSslAsync(int? companyId);
    }
}
