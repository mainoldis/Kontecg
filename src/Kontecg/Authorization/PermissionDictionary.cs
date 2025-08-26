using System.Collections.Generic;
using System.Linq;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Used to store and manipulate dictionary of permissions.
    /// </summary>
    public class PermissionDictionary : Dictionary<string, Permission>
    {
        /// <summary>
        ///     Adds all child permissions of current permissions recursively.
        /// </summary>
        public virtual void AddAllPermissions()
        {
            foreach (Permission permission in Values.ToList())
            {
                AddPermissionRecursively(permission);
            }
        }

        /// <summary>
        ///     Adds a permission and it's all child permissions to dictionary.
        /// </summary>
        /// <param name="permission">Permission to be added</param>
        private void AddPermissionRecursively(Permission permission)
        {
            //Prevent multiple adding of same named permission.
            if (TryGetValue(permission.Name, out Permission existingPermission))
            {
                if (existingPermission != permission)
                {
                    throw new KontecgInitializationException(
                        "Duplicate permission name detected for " + permission.Name);
                }
            }
            else
            {
                this[permission.Name] = permission;
            }

            //Add child permissions (recursive call)
            foreach (Permission childPermission in permission.Children)
            {
                AddPermissionRecursively(childPermission);
            }
        }
    }
}
