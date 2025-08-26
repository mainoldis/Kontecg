using Kontecg.Collections;

namespace Kontecg.Baseline.Configuration
{
    public class UserManagementConfig : IUserManagementConfig
    {
        public UserManagementConfig()
        {
            ExternalAuthenticationSources = new TypeList();
        }

        public ITypeList<object> ExternalAuthenticationSources { get; set; }
    }
}
