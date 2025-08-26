using System;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    public interface IKontecgServicioAdminModuleConfig
    {
        bool IsEnabled { get; }

        string CompanyName { get; }

        Type AuthenticationSourceType { get; }

        void Enable(Type authenticationSourceType, string companyName = KontecgCompanyBase.DefaultCompanyName);
    }
}
