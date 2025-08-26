using System.Linq;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Authorization
{
    public class PermissionManagerTesterTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Test_PermissionManager()
        {
            var authorizationConfiguration = new AuthorizationConfiguration();
            authorizationConfiguration.Providers.Add<MyAuthorizationProvider1>();
            authorizationConfiguration.Providers.Add<MyAuthorizationProvider2>();

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>().UsingFactoryMethod(() => new FeatureDependencyContext(LocalIocManager, Substitute.For<IFeatureChecker>())),
                Component.For<MyAuthorizationProvider1>().LifestyleTransient(),
                Component.For<MyAuthorizationProvider2>().LifestyleTransient(),
                Component.For<IUnitOfWorkManager, UnitOfWorkManager>().LifestyleTransient(),
                Component.For<ICurrentUnitOfWorkProvider, AsyncLocalCurrentUnitOfWorkProvider>().LifestyleTransient(),
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().LifestyleTransient(),
                Component.For<IMultiCompanyConfig, MultiCompanyConfig>().LifestyleTransient()
                );

            var permissionManager = new PermissionManager(LocalIocManager, authorizationConfiguration, LocalIocManager.Resolve<IUnitOfWorkManager>(), LocalIocManager.Resolve<IMultiCompanyConfig>());
            permissionManager.Initialize();

            permissionManager.GetAllPermissions().Count.ShouldBe(5);

            var userManagement = permissionManager.GetPermissionOrNull("Kontecg.Zero.Administration.UserManagement");
            userManagement.ShouldNotBe(null);
            userManagement.Children.Count.ShouldBe(1);

            var changePermissions = permissionManager.GetPermissionOrNull("Kontecg.Zero.Administration.UserManagement.ChangePermissions");
            changePermissions.ShouldNotBe(null);
            changePermissions.Parent.ShouldBeSameAs(userManagement);

            permissionManager.GetPermissionOrNull("NonExistingPermissionName").ShouldBe(null);

            userManagement.RemoveChildPermission(userManagement.Children.FirstOrDefault()?.Name);
            userManagement.Children.Count.ShouldBe(0);

            permissionManager.RemovePermission("Kontecg.Zero.Administration");
            permissionManager.GetPermissionOrNull("Kontecg.Zero.Administration").ShouldBe(null);
        }
        [Fact]
        public void Should_Manage_Permission_With_Custom_Properties()
        {
            var authorizationConfiguration = new AuthorizationConfiguration();
            authorizationConfiguration.Providers.Add<MyAuthorizationProviderWithCustomProperties>();

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>()
                    .UsingFactoryMethod(() => new FeatureDependencyContext(LocalIocManager, Substitute.For<IFeatureChecker>())),
                Component.For<MyAuthorizationProviderWithCustomProperties>().LifestyleTransient(),
                Component.For<IUnitOfWorkManager, UnitOfWorkManager>().LifestyleTransient(),
                Component.For<ICurrentUnitOfWorkProvider, AsyncLocalCurrentUnitOfWorkProvider>().LifestyleTransient(),
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().LifestyleTransient(),
                Component.For<IMultiCompanyConfig, MultiCompanyConfig>().LifestyleTransient()
            );

            var permissionManager = new PermissionManager(LocalIocManager, authorizationConfiguration, LocalIocManager.Resolve<IUnitOfWorkManager>(), LocalIocManager.Resolve<IMultiCompanyConfig>());
            permissionManager.Initialize();

            permissionManager.GetAllPermissions().Count.ShouldBe(4);

            var customPermission = permissionManager.GetPermissionOrNull("Kontecg.Zero.MyCustomPermission");
            customPermission.ShouldNotBe(null);
            customPermission.Children.Count.ShouldBe(2);

            customPermission.Properties.Count.ShouldBe(2);
            customPermission["MyProp1"].ShouldBe("Test");
            ((MyAuthorizationProviderWithCustomProperties.MyTestPropertyClass)customPermission["MyProp2"]).Prop1.ShouldBe("Test");

            //its not exist
            customPermission["MyProp3"].ShouldBeNull();

            customPermission.Children[0]["MyProp1"].ShouldBeNull();
            customPermission.Children[1]["MyProp1"].ShouldBe("TestChild");

            var customPermission2 = permissionManager.GetPermissionOrNull("Kontecg.Zero.MyCustomPermission2");
            customPermission2.ShouldNotBe(null);
            customPermission2.Children.Count.ShouldBe(0);

            customPermission2.Properties.Count.ShouldBe(0);
            customPermission2["MyProp1"].ShouldBeNull();

            customPermission2["MyProp1"] = "Test";

            var customPermission21 = permissionManager.GetPermissionOrNull("Kontecg.Zero.MyCustomPermission2");
            customPermission2.ShouldBeSameAs(customPermission21);

            customPermission21["MyProp1"].ShouldBe("Test");

        }
    }
}
