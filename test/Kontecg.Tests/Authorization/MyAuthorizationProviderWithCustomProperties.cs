using System.Collections.Generic;
using Kontecg.Authorization;
using Kontecg.Localization;

namespace Kontecg.Tests.Authorization
{
    public class MyAuthorizationProviderWithCustomProperties : AuthorizationProvider
    {
        public class MyTestPropertyClass
        {
            public string Prop1 { get; set; }
        }
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var myPermission = context.CreatePermission("Kontecg.Zero.MyCustomPermission",
                new FixedLocalizableString("Administration"),
                properties: new Dictionary<string, object>()
                {
                    {"MyProp1", "Test"},
                    {"MyProp2", new MyTestPropertyClass {Prop1 = "Test"}}
                }
            );
            //add children to permission
            myPermission.CreateChildPermission("Kontecg.Zero.MyCustomChildPermission",
                new FixedLocalizableString("Role management")
            );
            myPermission.CreateChildPermission("Kontecg.Zero.MyCustomChildPermission2",
                new FixedLocalizableString("Role management"),
                properties: new Dictionary<string, object>()
                {
                    {"MyProp1", "TestChild"},
                    {"MyProp2", new MyTestPropertyClass {Prop1 = "TestChild"}}
                });

            var myPermission2 = context.CreatePermission("Kontecg.Zero.MyCustomPermission2",
                new FixedLocalizableString("Administration")
            );
        }
    }
}