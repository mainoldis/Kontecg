using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Baseline;
using Kontecg.Baseline.Configuration;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;
using Kontecg.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Kontecg.Authorization.Roles
{
    public class KontecgRoleManager<TRole, TUser> : RoleManager<TRole>, IDomainService
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

        private readonly IPermissionManager _permissionManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public KontecgRoleManager(
            KontecgRoleStore<TRole, TUser> store,
            IEnumerable<IRoleValidator<TRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<KontecgRoleManager<TRole, TUser>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors,
                logger)
        {
            _permissionManager = permissionManager;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;

            RoleManagementConfig = roleManagementConfig;
            _organizationUnitRepository = organizationUnitRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            KontecgStore = store;
            KontecgSession = NullKontecgSession.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        public ILocalizationManager LocalizationManager { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public IRoleManagementConfig RoleManagementConfig { get; }

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        protected string LocalizationSourceName { get; set; }

        protected KontecgRoleStore<TRole, TUser> KontecgStore { get; }

        private IRolePermissionStore<TRole> RolePermissionStore
        {
            get
            {
                if (!(Store is IRolePermissionStore<TRole>))
                {
                    throw new KontecgException("Store is not IRolePermissionStore");
                }

                return Store as IRolePermissionStore<TRole>;
            }
        }

        /// <summary>
        ///     Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleName">The role's name to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(string roleName, string permissionName)
        {
            return await IsGrantedAsync((await GetRoleByNameAsync(roleName)).Id,
                _permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        ///     Checks if a role has a permission.
        /// </summary>
        /// <param name="roleId">The role's id to check it's permission</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, string permissionName)
        {
            return await IsGrantedAsync(roleId, _permissionManager.GetPermission(permissionName));
        }

        /// <summary>
        ///     Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public Task<bool> IsGrantedAsync(TRole role, Permission permission)
        {
            return IsGrantedAsync(role.Id, permission);
        }

        /// <summary>
        ///     Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleId">role id</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            RolePermissionCacheItem cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        /// <summary>
        ///     Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleId">role id</param>
        /// <param name="permission">The permission</param>
        /// <returns>True, if the role has the permission</returns>
        public virtual bool IsGranted(int roleId, Permission permission)
        {
            //Get cached role permissions
            RolePermissionCacheItem cacheItem = GetRolePermissionCacheItem(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        /// <summary>
        ///     Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(int roleId)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId));
        }

        /// <summary>
        ///     Gets granted permission names for a role.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(string roleName)
        {
            return await GetGrantedPermissionsAsync(await GetRoleByNameAsync(roleName));
        }

        /// <summary>
        ///     Gets granted permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TRole role)
        {
            RolePermissionCacheItem cacheItem = await GetRolePermissionCacheItemAsync(role.Id);
            IReadOnlyList<Permission> allPermissions = _permissionManager.GetAllPermissions();
            return allPermissions.Where(x => cacheItem.GrantedPermissions.Contains(x.Name)).ToList();
        }

        /// <summary>
        ///     Sets all granted permissions of a role at once.
        ///     Prohibits all other permissions.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(int roleId, IEnumerable<Permission> permissions)
        {
            await SetGrantedPermissionsAsync(await GetRoleByIdAsync(roleId), permissions);
        }

        /// <summary>
        ///     Sets all granted permissions of a role at once.
        ///     Prohibits all other permissions.
        /// </summary>
        /// <param name="role">The role</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(TRole role, IEnumerable<Permission> permissions)
        {
            IReadOnlyList<Permission> oldPermissions = await GetGrantedPermissionsAsync(role);
            Permission[] newPermissions = permissions.ToArray();

            foreach (Permission permission in oldPermissions.Where(p =>
                         !newPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await ProhibitPermissionAsync(role, permission);
            }

            foreach (Permission permission in newPermissions.Where(p =>
                         !oldPermissions.Contains(p, PermissionEqualityComparer.Instance)))
            {
                await GrantPermissionAsync(role, permission);
            }
        }

        /// <summary>
        ///     Grants a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task GrantPermissionAsync(TRole role, Permission permission)
        {
            if (await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        ///     Prohibits a permission for a role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="permission">Permission</param>
        public async Task ProhibitPermissionAsync(TRole role, Permission permission)
        {
            if (!await IsGrantedAsync(role.Id, permission))
            {
                return;
            }

            await RolePermissionStore.RemovePermissionAsync(role, new PermissionGrantInfo(permission.Name, true));
            await RolePermissionStore.AddPermissionAsync(role, new PermissionGrantInfo(permission.Name, false));
        }

        /// <summary>
        ///     Prohibits all permissions for a role.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ProhibitAllPermissionsAsync(TRole role)
        {
            foreach (Permission permission in _permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(role, permission);
            }
        }

        /// <summary>
        ///     Resets all permission settings for a role.
        ///     It removes all permission settings for the role.
        ///     Role will have permissions for which <see cref="StaticRoleDefinition.IsGrantedByDefault" /> returns true.
        /// </summary>
        /// <param name="role">Role</param>
        public async Task ResetAllPermissionsAsync(TRole role)
        {
            await RolePermissionStore.RemoveAllPermissionSettingsAsync(role);
        }

        /// <summary>
        ///     Creates a role.
        /// </summary>
        /// <param name="role">Role</param>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            IdentityResult result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            int? companyId = GetCurrentCompanyId();
            if (companyId.HasValue && !role.CompanyId.HasValue)
            {
                role.CompanyId = companyId.Value;
            }

            return await base.CreateAsync(role);
        }

        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            IdentityResult result = await CheckDuplicateRoleNameAsync(role.Id, role.Name, role.DisplayName);
            if (!result.Succeeded)
            {
                return result;
            }

            return await base.UpdateAsync(role);
        }

        /// <summary>
        ///     Deletes a role.
        /// </summary>
        /// <param name="role">Role</param>
        public override async Task<IdentityResult> DeleteAsync(TRole role)
        {
            if (role.IsStatic)
            {
                throw new UserFriendlyException(string.Format(L("CanNotDeleteStaticRole"), role.Name));
            }

            return await base.DeleteAsync(role);
        }

        /// <summary>
        ///     Gets a role by given id.
        ///     Throws exception if no role with given id.
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>Role</returns>
        /// <exception cref="KontecgException">Throws exception if no role with given id</exception>
        public virtual async Task<TRole> GetRoleByIdAsync(int roleId)
        {
            TRole role = await FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new KontecgException("There is no role with id: " + roleId);
            }

            return role;
        }

        /// <summary>
        ///     Gets a role by given name.
        ///     Throws exception if no role with given roleName.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role</returns>
        /// <exception cref="KontecgException">Throws exception if no role with given roleName</exception>
        public virtual async Task<TRole> GetRoleByNameAsync(string roleName)
        {
            TRole role = await FindByNameAsync(roleName);
            if (role == null)
            {
                throw new KontecgException("There is no role with name: " + roleName);
            }

            return role;
        }

        /// <summary>
        ///     Gets a role by given name.
        ///     Throws exception if no role with given roleName.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role</returns>
        /// <exception cref="KontecgException">Throws exception if no role with given roleName</exception>
        public virtual TRole GetRoleByName(string roleName)
        {
            string normalizedRoleName = roleName.ToUpperInvariant();

            TRole role = KontecgStore.FindByName(normalizedRoleName);
            if (role == null)
            {
                throw new KontecgException("There is no role with name: " + roleName);
            }

            return role;
        }

        public async Task GrantAllPermissionsAsync(TRole role)
        {
            FeatureDependencyContext.CompanyId = role.CompanyId;

            IEnumerable<Permission> permissions = _permissionManager.GetAllPermissions(role.GetMultiCompanySide())
                .Where(permission =>
                    permission.FeatureDependency == null ||
                    permission.FeatureDependency.IsSatisfied(FeatureDependencyContext)
                );

            await SetGrantedPermissionsAsync(role, permissions);
        }

        public virtual async Task<IdentityResult> CreateStaticRolesAsync(int companyId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                IEnumerable<StaticRoleDefinition> staticRoleDefinitions = RoleManagementConfig.StaticRoles.Where(
                    sr => sr.Side == MultiCompanySides.Company
                );

                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    foreach (StaticRoleDefinition staticRoleDefinition in staticRoleDefinitions)
                    {
                        TRole role = MapStaticRoleDefinitionToRole(companyId, staticRoleDefinition);

                        IdentityResult identityResult = await CreateAsync(role);
                        if (!identityResult.Succeeded)
                        {
                            return identityResult;
                        }
                    }
                }

                return IdentityResult.Success;
            });
        }

        public virtual async Task<IdentityResult> CheckDuplicateRoleNameAsync(
            int? expectedRoleId,
            string name,
            string displayName)
        {
            TRole role = await FindByNameAsync(name);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleNameIsAlreadyTaken"), name));
            }

            role = await FindByDisplayNameAsync(displayName);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException(string.Format(L("RoleDisplayNameIsAlreadyTaken"), displayName));
            }

            return IdentityResult.Success;
        }

        /// <summary>
        ///     Gets roles of a given organizationUnit
        /// </summary>
        /// <param name="organizationUnit">OrganizationUnit to get belonging roles </param>
        /// <param name="includeChildren">Includes roles for children organization units to result when true. Default is false</param>
        /// <returns></returns>
        public virtual async Task<List<TRole>> GetRolesInOrganizationUnitAsync(
            OrganizationUnit organizationUnit,
            bool includeChildren = false)
        {
            List<TRole> result = _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (!includeChildren)
                {
                    IQueryable<TRole> query = from organizationUnitRole in _organizationUnitRoleRepository.GetAll()
                        join role in Roles on organizationUnitRole.RoleId equals role.Id
                        where organizationUnitRole.OrganizationUnitId == organizationUnit.Id
                        select role;

                    return query.ToList();
                }
                else
                {
                    IQueryable<TRole> query = from organizationUnitRole in _organizationUnitRoleRepository.GetAll()
                        join role in Roles on organizationUnitRole.RoleId equals role.Id
                        join ou in _organizationUnitRepository.GetAll() on organizationUnitRole.OrganizationUnitId
                            equals
                            ou.Id
                        where ou.Code.StartsWith(organizationUnit.Code)
                        select role;

                    return query.ToList();
                }
            });

            return await Task.FromResult(result);
        }

        public virtual async Task SetOrganizationUnitsAsync(int roleId, params long[] organizationUnitIds)
        {
            await SetOrganizationUnitsAsync(
                await GetRoleByIdAsync(roleId),
                organizationUnitIds
            );
        }

        public virtual async Task SetOrganizationUnitsAsync(TRole role, params long[] organizationUnitIds)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                organizationUnitIds ??= new long[0];

                List<OrganizationUnit> currentOus = await GetOrganizationUnitsAsync(role);

                //Remove from removed OUs
                foreach (OrganizationUnit currentOu in currentOus)
                {
                    if (!organizationUnitIds.Contains(currentOu.Id))
                    {
                        await RemoveFromOrganizationUnitAsync(role, currentOu);
                    }
                }

                //Add to added OUs
                foreach (long organizationUnitId in organizationUnitIds)
                {
                    if (currentOus.All(ou => ou.Id != organizationUnitId))
                    {
                        await AddToOrganizationUnitAsync(
                            role,
                            await _organizationUnitRepository.GetAsync(organizationUnitId)
                        );
                    }
                }
            });
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(int roleId, long ouId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await IsInOrganizationUnitAsync(
                    await GetRoleByIdAsync(roleId),
                    await _organizationUnitRepository.GetAsync(ouId)
                )
            );
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _organizationUnitRoleRepository.CountAsync(uou =>
                    uou.RoleId == role.Id && uou.OrganizationUnitId == ou.Id
                ) > 0;
            });
        }

        public virtual async Task AddToOrganizationUnitAsync(int roleId, long ouId, int? companyId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await AddToOrganizationUnitAsync(
                    await GetRoleByIdAsync(roleId),
                    await _organizationUnitRepository.GetAsync(ouId)
                );
            });
        }

        public virtual async Task AddToOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (await IsInOrganizationUnitAsync(role, ou))
                {
                    return;
                }

                await _organizationUnitRoleRepository.InsertAsync(new OrganizationUnitRole(role.CompanyId, role.Id,
                    ou.Id));
            });
        }

        public async Task RemoveFromOrganizationUnitAsync(int roleId, long organizationUnitId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await RemoveFromOrganizationUnitAsync(
                    await GetRoleByIdAsync(roleId),
                    await _organizationUnitRepository.GetAsync(organizationUnitId)
                );
            });
        }

        public virtual async Task RemoveFromOrganizationUnitAsync(TRole role, OrganizationUnit ou)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _organizationUnitRoleRepository.DeleteAsync(uor =>
                    uor.RoleId == role.Id && uor.OrganizationUnitId == ou.Id
                );
            });
        }

        public virtual async Task<List<OrganizationUnit>> GetOrganizationUnitsAsync(TRole role)
        {
            List<OrganizationUnit> result = _unitOfWorkManager.WithUnitOfWork(() =>
            {
                IQueryable<OrganizationUnit> query = from uor in _organizationUnitRoleRepository.GetAll()
                    join ou in _organizationUnitRepository.GetAll() on uor.OrganizationUnitId equals ou.Id
                    where uor.RoleId == role.Id
                    select ou;

                return query.ToList();
            });

            return await Task.FromResult(result);
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }

        protected virtual TRole MapStaticRoleDefinitionToRole(int companyId, StaticRoleDefinition staticRoleDefinition)
        {
            return new TRole
            {
                CompanyId = companyId,
                Name = staticRoleDefinition.RoleName,
                DisplayName = staticRoleDefinition.RoleDisplayName,
                IsStatic = true
            };
        }

        private Task<TRole> FindByDisplayNameAsync(string displayName)
        {
            return KontecgStore.FindByDisplayNameAsync(displayName);
        }

        private async Task<RolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            string cacheKey = roleId + "@" + (GetCurrentCompanyId() ?? 0);
            return await _cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                RolePermissionCacheItem newCacheItem = new RolePermissionCacheItem(roleId);

                TRole role = await Store.FindByIdAsync(roleId.ToString(), CancellationToken);
                if (role == null)
                {
                    throw new KontecgException("There is no role with given id: " + roleId);
                }

                StaticRoleDefinition staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                    r.RoleName == role.Name && r.Side == role.GetMultiCompanySide()
                );

                if (staticRoleDefinition != null)
                {
                    foreach (Permission permission in _permissionManager.GetAllPermissions())
                    {
                        if (staticRoleDefinition.IsGrantedByDefault(permission))
                        {
                            newCacheItem.GrantedPermissions.Add(permission.Name);
                        }
                    }
                }

                foreach (PermissionGrantInfo permissionInfo in await RolePermissionStore.GetPermissionsAsync(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.AddIfNotContains(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.GrantedPermissions.Remove(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private RolePermissionCacheItem GetRolePermissionCacheItem(int roleId)
        {
            string cacheKey = roleId + "@" + (GetCurrentCompanyId() ?? 0);
            return _cacheManager.GetRolePermissionCache().Get(cacheKey, () =>
            {
                RolePermissionCacheItem newCacheItem = new RolePermissionCacheItem(roleId);

                TRole role = KontecgStore.FindById(roleId.ToString(), CancellationToken);
                if (role == null)
                {
                    throw new KontecgException("There is no role with given id: " + roleId);
                }

                StaticRoleDefinition staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                    r.RoleName == role.Name && r.Side == role.GetMultiCompanySide()
                );

                if (staticRoleDefinition != null)
                {
                    foreach (Permission permission in _permissionManager.GetAllPermissions())
                    {
                        if (staticRoleDefinition.IsGrantedByDefault(permission))
                        {
                            newCacheItem.GrantedPermissions.Add(permission.Name);
                        }
                    }
                }

                foreach (PermissionGrantInfo permissionInfo in RolePermissionStore.GetPermissions(roleId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.AddIfNotContains(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.GrantedPermissions.Remove(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private int? GetCurrentCompanyId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }
    }
}
