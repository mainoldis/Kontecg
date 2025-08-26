using System;

namespace Kontecg.Baseline.Ldap.Configuration
{
    public interface IKontecgLdapModuleConfig
    {
        bool IsEnabled { get; }

        /// <summary>
        ///     Otherwise SamAccountName will be used as a username
        /// </summary>
        bool UseUserPrincipalNameAsUserName { get; set; }

        Type AuthenticationSourceType { get; }

        void Enable(Type authenticationSourceType);
    }
}
