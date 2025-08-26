using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Application.Navigation;
using Kontecg.Authorization;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Kontecg.Tests.Application.Navigation
{
    internal class NavigationTestCase
    {
        public NavigationManager NavigationManager { get; private set; }

        public UserNavigationManager UserNavigationManager { get; private set; }

        private readonly IIocManager _iocManager;

        public NavigationTestCase()
            : this(new IocManager())
        {
        }

        public NavigationTestCase(IIocManager iocManager)
        {
            _iocManager = iocManager;
            Initialize();
        }

        private void Initialize()
        {
            //Navigation providers should be registered
            _iocManager.Register<MyNavigationProvider1>();
            _iocManager.Register<MyNavigationProvider2>();

            //Preparing navigation configuration
            var configuration = new NavigationConfiguration();
            configuration.Providers.Add<MyNavigationProvider1>();
            configuration.Providers.Add<MyNavigationProvider2>();

            //Initializing navigation manager
            NavigationManager = new NavigationManager(_iocManager, configuration);
            NavigationManager.Initialize();

            _iocManager.IocContainer.Register(
                Component.For<IPermissionDependencyContext, PermissionDependencyContext>()
                    .UsingFactoryMethod(
                        () => new PermissionDependencyContext(_iocManager)
                        {
                            PermissionChecker = CreateMockPermissionChecker()
                        })
                );

            _iocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>()
                    .UsingFactoryMethod(
                        () => new FeatureDependencyContext(_iocManager, Substitute.For<IFeatureChecker>()))
                );

            //Create user navigation manager to test
            UserNavigationManager = new UserNavigationManager(NavigationManager, Substitute.For<ILocalizationContext>(), _iocManager);
        }

        private static IPermissionChecker CreateMockPermissionChecker()
        {
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync(new UserIdentifier(1, 1), "Kontecg.Zero.UserManagement").Returns(Task.FromResult(true));
            permissionChecker.IsGrantedAsync(new UserIdentifier(1, 1), "Kontecg.Zero.RoleManagement").Returns(Task.FromResult(false));
            return permissionChecker;
        }

        public class MyNavigationProvider1 : NavigationProvider
        {
            public override void SetNavigation(INavigationProviderContext context)
            {
                context.Manager.MainMenu.AddItem(
                    new MenuItemDefinition(
                        "Kontecg.Zero.Administration",
                        new FixedLocalizableString("Administration"),
                        "fa fa-asterisk",
                        requiresAuthentication: true
                        ).AddItem(
                            new MenuItemDefinition(
                                "Kontecg.Zero.Administration.User",
                                new FixedLocalizableString("User management"),
                                "fa fa-users",
                                resourceType: null,
                                "#/admin/users",
                                permissionDependency: new SimplePermissionDependency("Kontecg.Zero.UserManagement"),
                                customData: "A simple test data"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Kontecg.Zero.Administration.Role",
                                new FixedLocalizableString("Role management"),
                                "fa fa-star-o",
                                resourceType: null,
                                "#/admin/roles",
                                permissionDependency: new SimplePermissionDependency("Kontecg.Zero.RoleManagement")
                                )
                        )
                    );
            }
        }

        public class MyNavigationProvider2 : NavigationProvider
        {
            public override void SetNavigation(INavigationProviderContext context)
            {
                var adminMenu = context.Manager.MainMenu.GetItemByName("Kontecg.Zero.Administration");
                adminMenu.AddItem(
                    new MenuItemDefinition(
                        "Kontecg.Zero.Administration.Setting",
                        new FixedLocalizableString("Setting management"),
                        icon: "fa fa-cog",
                        url: "#/admin/settings",
                        customData: new MyCustomDataClass { Data1 = 42, Data2 = "FortyTwo" }
                        )
                    );
            }
        }

        public class MyCustomDataClass
        {
            public int Data1 { get; set; }

            public string Data2 { get; set; }
        }
    }
}
