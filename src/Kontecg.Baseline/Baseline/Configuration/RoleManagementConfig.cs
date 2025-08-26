using System.Collections.Generic;

namespace Kontecg.Baseline.Configuration
{
    internal class RoleManagementConfig : IRoleManagementConfig
    {
        public RoleManagementConfig()
        {
            StaticRoles = new List<StaticRoleDefinition>();
        }

        public List<StaticRoleDefinition> StaticRoles { get; }
    }
}
