using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;

namespace Kontecg.Application.Navigation
{
    /// <summary>
    /// Manages user-specific navigation menus and items.
    /// </summary>
    /// <remarks>
    /// Provides methods to retrieve and build navigation menus for users, handling permissions and feature dependencies.
    /// </remarks>
    internal class UserNavigationManager : IUserNavigationManager, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ILocalizationContext _localizationContext;

        private readonly INavigationManager _navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNavigationManager"/> class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager instance.</param>
        /// <param name="localizationContext">The localization context.</param>
        /// <param name="iocResolver">The IoC resolver for dependency injection.</param>
        public UserNavigationManager(
            INavigationManager navigationManager,
            ILocalizationContext localizationContext,
            IIocResolver iocResolver)
        {
            _navigationManager = navigationManager;
            _localizationContext = localizationContext;
            _iocResolver = iocResolver;
            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        /// Gets or sets the current session for the user.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        /// <summary>
        /// Asynchronously gets the navigation menu for a specific user.
        /// </summary>
        /// <param name="menuName">The name of the menu to retrieve.</param>
        /// <param name="user">The user identifier.</param>
        /// <returns>The user menu for the specified menu name and user.</returns>
        /// <exception cref="KontecgException">Thrown if the menu with the given name does not exist.</exception>
        public async Task<UserMenu> GetMenuAsync(string menuName, UserIdentifier user)
        {
            MenuDefinition menuDefinition = _navigationManager.Menus.GetOrDefault(menuName);
            if (menuDefinition == null)
            {
                throw new KontecgException("There is no menu with given name: " + menuName);
            }

            UserMenu userMenu = new UserMenu(menuDefinition, _localizationContext);
            await FillUserMenuItemsAsync(user, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        /// <summary>
        /// Asynchronously gets all navigation menus for a specific user.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <returns>A read-only list of user menus.</returns>
        public async Task<IReadOnlyList<UserMenu>> GetMenusAsync(UserIdentifier user)
        {
            List<UserMenu> userMenus = new List<UserMenu>();

            foreach (MenuDefinition menu in _navigationManager.Menus.Values)
            {
                userMenus.Add(await GetMenuAsync(menu.Name, user));
            }

            return userMenus;
        }

        /// <summary>
        /// Recursively fills the user menu items based on permissions and feature dependencies.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="menuItemDefinitions">The list of menu item definitions.</param>
        /// <param name="userMenuItems">The list to populate with user menu items.</param>
        /// <returns>The number of menu items added.</returns>
        private async Task<int> FillUserMenuItemsAsync(UserIdentifier user,
            IList<MenuItemDefinition> menuItemDefinitions,
            IList<UserMenuItem> userMenuItems)
        {
            //TODO: Can be optimized by re-using FeatureDependencyContext.

            int addedMenuItemCount = 0;

            using IScopedIocResolver scope = _iocResolver.CreateScope();
            PermissionDependencyContext permissionDependencyContext = scope.Resolve<PermissionDependencyContext>();
            permissionDependencyContext.User = user;

            FeatureDependencyContext featureDependencyContext = scope.Resolve<FeatureDependencyContext>();
            featureDependencyContext.CompanyId = user == null ? null : user.CompanyId;

            foreach (MenuItemDefinition menuItemDefinition in menuItemDefinitions)
            {
                if (menuItemDefinition.RequiresAuthentication && user == null)
                {
                    continue;
                }

                if (menuItemDefinition.PermissionDependency != null &&
                    (user == null ||
                     !await menuItemDefinition.PermissionDependency.IsSatisfiedAsync(permissionDependencyContext)))
                {
                    continue;
                }

                if (menuItemDefinition.FeatureDependency != null &&
                    (KontecgSession.MultiCompanySide == MultiCompanySides.Company ||
                     (user != null && user.CompanyId != null)) &&
                    !await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext))
                {
                    continue;
                }

                UserMenuItem userMenuItem = new UserMenuItem(menuItemDefinition, _localizationContext);
                if (menuItemDefinition.IsLeaf ||
                    await FillUserMenuItemsAsync(user, menuItemDefinition.Items, userMenuItem.Items) > 0)
                {
                    userMenuItems.Add(userMenuItem);
                    ++addedMenuItemCount;
                }
            }

            return addedMenuItemCount;
        }
    }
}
