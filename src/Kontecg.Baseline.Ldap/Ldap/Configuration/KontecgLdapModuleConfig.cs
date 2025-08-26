using System;
using Kontecg.Baseline.Configuration;

namespace Kontecg.Baseline.Ldap.Configuration
{
    public class KontecgLdapModuleConfig : IKontecgLdapModuleConfig
    {
        private readonly IKontecgBaselineConfig _baselineConfig;

        public KontecgLdapModuleConfig(IKontecgBaselineConfig baselineConfig)
        {
            _baselineConfig = baselineConfig;
        }

        public bool IsEnabled { get; private set; }

        /// <inheritdoc />
        public bool UseUserPrincipalNameAsUserName { get; set; }

        public Type AuthenticationSourceType { get; private set; }

        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;

            _baselineConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}
