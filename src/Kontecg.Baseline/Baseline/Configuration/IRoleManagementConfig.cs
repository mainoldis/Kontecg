using System.Collections.Generic;

namespace Kontecg.Baseline.Configuration
{
    public interface IRoleManagementConfig
    {
        List<StaticRoleDefinition> StaticRoles { get; }
    }
}
