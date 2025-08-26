using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.Collections.Extensions;
using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization
{
    public abstract class PermissionDefinitionContextBase : IPermissionDefinitionContext
    {
        protected readonly PermissionDictionary Permissions;

        protected PermissionDefinitionContextBase()
        {
            Permissions = new PermissionDictionary();
        }

        public Permission CreatePermission(
            string name,
            ILocalizableString displayName = null,
            ILocalizableString description = null,
            MultiCompanySides multiCompanySides = MultiCompanySides.Host | MultiCompanySides.Company,
            IFeatureDependency featureDependency = null,
            Dictionary<string, object> properties = null)
        {
            if (Permissions.ContainsKey(name))
            {
                throw new KontecgException("There is already a permission with name: " + name);
            }

            Permission permission = new Permission(name, displayName, description, multiCompanySides, featureDependency,
                properties);
            Permissions[permission.Name] = permission;
            return permission;
        }

        public virtual Permission GetPermissionOrNull(string name)
        {
            return Permissions.GetOrDefault(name);
        }

        public virtual void RemovePermission(string name)
        {
            Permissions.Remove(name);
        }
    }
}
