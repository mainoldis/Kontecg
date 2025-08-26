using System;
using Kontecg.Baseline.Configuration;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    public class KontecgServicioAdminModuleConfig : IKontecgServicioAdminModuleConfig
    {
        private readonly IKontecgBaselineConfig _chenetConfig;

        public KontecgServicioAdminModuleConfig(IKontecgBaselineConfig chenetConfig)
        {
            _chenetConfig = chenetConfig;
        }

        public bool IsEnabled { get; private set; }

        public string CompanyName { get; private set; }

        public Type AuthenticationSourceType { get; private set; }

        public void Enable(Type authenticationSourceType, string companyName = KontecgCompanyBase.DefaultCompanyName)
        {
            AuthenticationSourceType = authenticationSourceType;
            CompanyName = companyName;
            IsEnabled = true;

            _chenetConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }

        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;

            _chenetConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}
