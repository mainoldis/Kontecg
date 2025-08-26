using Kontecg.Authorization;
using Kontecg.Localization;
using Shouldly;

namespace Kontecg.Tests.Authorization
{
    public class MyAuthorizationProvider2 : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Get existing root permission group 'Administration'
            var administration = context.GetPermissionOrNull("Kontecg.Zero.Administration");
            administration.ShouldNotBe(null);

            //Create 'Role management' permission under 'Administration' group
            var roleManegement = administration.CreateChildPermission("Kontecg.Zero.Administration.RoleManagement", new FixedLocalizableString("Role management"));

            //Create 'Create role' (to be able to create a new role) permission  as child of 'Role management' permission.
            roleManegement.CreateChildPermission("Kontecg.Zero.Administration.RoleManagement.CreateRole", new FixedLocalizableString("Create role"));
        }
    }
}