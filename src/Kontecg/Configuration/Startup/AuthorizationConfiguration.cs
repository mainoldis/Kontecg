using Kontecg.Authorization;
using Kontecg.Collections;

namespace Kontecg.Configuration.Startup
{
    internal class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public AuthorizationConfiguration()
        {
            Providers = new TypeList<AuthorizationProvider>();
            IsEnabled = true;
        }

        public ITypeList<AuthorizationProvider> Providers { get; }

        public bool IsEnabled { get; set; }
    }
}
