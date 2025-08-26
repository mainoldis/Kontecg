using Kontecg.Authorization;
using Kontecg.Localization;

namespace Kontecg.Tests.Authorization
{
    public class MyAuthorizationProvider1 : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Create a root permission group for 'Administration' permissions
            var administration = context.CreatePermission("Kontecg.Zero.Administration", new FixedLocalizableString("Administration"));

            //Create 'User management' permission under 'Administration' group
            var userManagement = administration.CreateChildPermission("Kontecg.Zero.Administration.UserManagement", new FixedLocalizableString("User management"));

            //Create 'Change permissions' (to be able to change permissions of a user) permission as child of 'User management' permission.
            userManagement.CreateChildPermission("Kontecg.Zero.Administration.UserManagement.ChangePermissions", new FixedLocalizableString("Change permissions"));
        }
    }
}