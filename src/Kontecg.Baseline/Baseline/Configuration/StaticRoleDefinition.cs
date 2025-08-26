using System.Collections.Generic;
using Kontecg.Authorization;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.Configuration
{
    public class StaticRoleDefinition
    {
        public StaticRoleDefinition(string roleName, MultiCompanySides side, bool grantAllPermissionsByDefault = false)
        {
            RoleName = roleName;
            RoleDisplayName = roleName;
            Side = side;
            GrantAllPermissionsByDefault = grantAllPermissionsByDefault;
            GrantedPermissions = new List<string>();
        }

        public StaticRoleDefinition(string roleName, string roleDisplayName, MultiCompanySides side,
            bool grantAllPermissionsByDefault = false)
        {
            RoleName = roleName;
            RoleDisplayName = roleDisplayName;
            Side = side;
            GrantAllPermissionsByDefault = grantAllPermissionsByDefault;
            GrantedPermissions = new List<string>();
        }

        public string RoleName { get; }

        public string RoleDisplayName { get; }

        public bool GrantAllPermissionsByDefault { get; set; }

        public List<string> GrantedPermissions { get; }

        public MultiCompanySides Side { get; }

        public virtual bool IsGrantedByDefault(Permission permission)
        {
            return GrantAllPermissionsByDefault || GrantedPermissions.Contains(permission.Name);
        }
    }
}
