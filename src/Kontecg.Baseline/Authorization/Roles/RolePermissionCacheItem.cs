using System;
using System.Collections.Generic;

namespace Kontecg.Authorization.Roles
{
    /// <summary>
    ///     Used to cache permissions of a role.
    /// </summary>
    [Serializable]
    public class RolePermissionCacheItem
    {
        public const string CacheStoreName = "KontecgBaselineRolePermissions";

        public RolePermissionCacheItem()
        {
            GrantedPermissions = new HashSet<string>();
        }

        public RolePermissionCacheItem(int roleId)
            : this()
        {
            RoleId = roleId;
        }

        public long RoleId { get; set; }

        public HashSet<string> GrantedPermissions { get; set; }
    }
}
