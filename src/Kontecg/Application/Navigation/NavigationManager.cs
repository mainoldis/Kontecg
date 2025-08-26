using System;
using System.Collections.Generic;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization;

namespace Kontecg.Application.Navigation
{
    /// <summary>
    /// Manages the application's navigation structure and menu definitions.
    /// This class is responsible for initializing and maintaining the navigation hierarchy,
    /// including menus and their associated navigation providers.
    /// </summary>
    /// <remarks>
    /// NavigationManager is registered as a singleton dependency, ensuring that the navigation
    /// structure is shared across the entire application. It maintains a collection of menu
    /// definitions and coordinates with navigation providers to build the complete navigation
    /// hierarchy. The class uses dependency injection to resolve navigation providers and
    /// configuration settings.
    /// </remarks>
    internal class NavigationManager : INavigationManager, ISingletonDependency
    {
        private readonly INavigationConfiguration _configuration;

        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the NavigationManager class.
        /// </summary>
        /// <param name="iocResolver">The IoC resolver used to resolve navigation providers.</param>
        /// <param name="configuration">The navigation configuration containing provider types and settings.</param>
        /// <remarks>
        /// The constructor initializes the navigation manager with the required dependencies
        /// and creates the default MainMenu definition. The MainMenu is automatically added
        /// to the menus collection and serves as the primary navigation container for the application.
        /// </remarks>
        public NavigationManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;

            Menus = new Dictionary<string, MenuDefinition>
            {
                {
                    "MainMenu",
                    new MenuDefinition("MainMenu",
                        new LocalizableString("MainMenu", KontecgConsts.LocalizationSourceName))
                }
            };
        }

        /// <summary>
        /// Gets the collection of menu definitions managed by this navigation manager.
        /// </summary>
        /// <value>
        /// A dictionary containing menu definitions indexed by their unique names.
        /// </value>
        /// <remarks>
        /// The Menus collection contains all menu definitions that have been registered
        /// with the navigation system. Each menu has a unique name that serves as its
        /// identifier and can be used to retrieve specific menus for rendering or manipulation.
        /// </remarks>
        public IDictionary<string, MenuDefinition> Menus { get; }

        /// <summary>
        /// Gets the main menu definition for the application.
        /// </summary>
        /// <value>
        /// The MenuDefinition for the main navigation menu.
        /// </value>
        /// <remarks>
        /// The MainMenu property provides convenient access to the primary navigation menu
        /// of the application. This menu typically contains the main navigation items and
        /// serves as the root container for the application's navigation hierarchy.
        /// </remarks>
        public MenuDefinition MainMenu => Menus["MainMenu"];

        /// <summary>
        /// Initializes the navigation system by resolving and configuring all navigation providers.
        /// </summary>
        /// <remarks>
        /// This method is responsible for setting up the complete navigation hierarchy by
        /// iterating through all registered navigation providers and allowing them to
        /// configure their respective navigation items. Each provider is resolved from the
        /// IoC container and given a context to work with. The method uses disposable wrappers
        /// to ensure proper resource management when resolving providers.
        /// </remarks>
        public void Initialize()
        {
            NavigationProviderContext context = new NavigationProviderContext(this);

            foreach (Type providerType in _configuration.Providers)
            {
                using IDisposableDependencyObjectWrapper<NavigationProvider> provider =
                    _iocResolver.ResolveAsDisposable<NavigationProvider>(providerType);
                provider.Object.SetNavigation(context);
            }
        }
    }
}
