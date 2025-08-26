using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization.Roles;
using Kontecg.Baseline;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;
using Kontecg.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kontecg.Authorization.Users
{
    public class KontecgUserManager<TRole, TUser> : UserManager<TUser>, IDomainService
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        protected IUserPermissionStore<TUser> UserPermissionStore
        {
            get
            {
                if (!(Store is IUserPermissionStore<TUser>))
                {
                    throw new KontecgException("Store is not IUserPermissionStore");
                }

                return Store as IUserPermissionStore<TUser>;
            }
        }

        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        protected KontecgRoleManager<TRole, TUser> RoleManager { get; }

        protected KontecgUserStore<TRole, TUser> KontecgUserStore { get; }

        public IMultiCompanyConfig MultiCompany { get; set; }

        private readonly IPermissionManager _permissionManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly ISettingManager _settingManager;
        private readonly IOptions<IdentityOptions> _optionsAccessor;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;

        public KontecgUserManager(
            KontecgRoleManager<TRole, TUser> roleManager,
            KontecgUserStore<TRole, TUser> userStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            ISettingManager settingManager,
            IRepository<UserLogin, long> userLoginRepository)
            : base(
                userStore,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            _permissionManager = permissionManager;
            _unitOfWorkManager = unitOfWorkManager;
            _cacheManager = cacheManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _settingManager = settingManager;
            _userLoginRepository = userLoginRepository;
            _optionsAccessor = optionsAccessor;

            KontecgUserStore = userStore;
            RoleManager = roleManager;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        public virtual Task<IQueryable<TUser>> GetUsersAsync()
            => KontecgUserStore.GetUsersAsync();

        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress);
            if (!result.Succeeded)
            {
                return result;
            }

            var companyId = GetCurrentCompanyId();
            if (companyId.HasValue && !user.CompanyId.HasValue)
            {
                user.CompanyId = companyId.Value;
            }

            await InitializeOptionsAsync(user.CompanyId);

            return await base.CreateAsync(user);
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permissionName">Permission name</param>
        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await IsGrantedAsync(
                userId,
                _permissionManager.GetPermission(permissionName)
            );
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permissionName">Permission name</param>
        public virtual bool IsGranted(long userId, string permissionName)
        {
            return IsGranted(
                userId,
                _permissionManager.GetPermission(permissionName)
            );
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual Task<bool> IsGrantedAsync(TUser user, Permission permission)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return IsGrantedAsync(user.Id, permission);
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual bool IsGranted(TUser user, Permission permission)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return IsGranted(user.Id, permission);
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual async Task<bool> IsGrantedAsync(long userId, Permission permission)
        {
            //Check for multi-tenancy side
            if (!permission.MultiCompanySides.HasFlag(GetCurrentMultiCompanySide()))
            {
                return false;
            }

            //Check for depended features
            if (permission.FeatureDependency != null && GetCurrentMultiCompanySide() == MultiCompanySides.Company)
            {
                FeatureDependencyContext.CompanyId = GetCurrentCompanyId();

                if (!await permission.FeatureDependency.IsSatisfiedAsync(FeatureDependencyContext))
                {
                    return false;
                }
            }

            //Get cached user permissions
            var cacheItem = await GetUserPermissionCacheItemAsync(userId);
            if (cacheItem == null)
            {
                return false;
            }

            //Check for user-specific value
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
            {
                return true;
            }

            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
            {
                return false;
            }

            //Check for roles
            foreach (var roleId in cacheItem.RoleIds)
            {
                if (await RoleManager.IsGrantedAsync(roleId, permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual bool IsGranted(long userId, Permission permission)
        {
            //Check for multi-tenancy side
            if (!permission.MultiCompanySides.HasFlag(GetCurrentMultiCompanySide()))
            {
                return false;
            }

            //Check for depended features
            if (permission.FeatureDependency != null && GetCurrentMultiCompanySide() == MultiCompanySides.Company)
            {
                FeatureDependencyContext.CompanyId = GetCurrentCompanyId();

                if (!permission.FeatureDependency.IsSatisfied(FeatureDependencyContext))
                {
                    return false;
                }
            }

            //Get cached user permissions
            var cacheItem = GetUserPermissionCacheItem(userId);
            if (cacheItem == null)
            {
                return false;
            }

            //Check for user-specific value
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
            {
                return true;
            }

            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
            {
                return false;
            }

            //Check for roles
            foreach (var roleId in cacheItem.RoleIds)
            {
                if (RoleManager.IsGranted(roleId, permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets granted permissions for a user.
        /// </summary>
        /// <param name="user">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TUser user)
        {
            var permissionList = new List<Permission>();

            foreach (var permission in await _permissionManager.GetAllPermissionsAsync())
            {
                if (await IsGrantedAsync(user.Id, permission))
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a user at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(TUser user, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(user);
            var newPermissions = permissions.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p)))
            {
                await ProhibitPermissionAsync(user, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p)))
            {
                await GrantPermissionAsync(user, permission);
            }
        }

        /// <summary>
        /// Prohibits all permissions for a user.
        /// </summary>
        /// <param name="user">User</param>
        public async Task ProhibitAllPermissionsAsync(TUser user)
        {
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(user, permission);
            }
        }

        /// <summary>
        /// Resets all permission settings for a user.
        /// It removes all permission settings for the user.
        /// User will have permissions according to his roles.
        /// This method does not prohibit all permissions.
        /// For that, use <see cref="ProhibitAllPermissionsAsync"/>.
        /// </summary>
        /// <param name="user">User</param>
        public async Task ResetAllPermissionsAsync(TUser user)
        {
            await UserPermissionStore.RemoveAllPermissionSettingsAsync(user);
        }

        /// <summary>
        /// Resets all permission settings for a user.
        /// It removes all permission settings for the user.
        /// User will have permissions according to his roles.
        /// This method does not prohibit all permissions.
        /// For that, use <see cref="ProhibitAllPermissionsAsync"/>.
        /// </summary>
        /// <param name="user">User</param>
        public void ResetAllPermissions(TUser user)
        {
            UserPermissionStore.RemoveAllPermissionSettings(user);
        }

        /// <summary>
        /// Grants a permission for a user if not already granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task GrantPermissionAsync(TUser user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, false));

            if (await IsGrantedAsync(user.Id, permission))
            {
                return;
            }

            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        /// Prohibits a permission for a user if it's granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task ProhibitPermissionAsync(TUser user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, true));

            if (!await IsGrantedAsync(user.Id, permission))
            {
                return;
            }

            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, false));
        }

        public virtual Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            return KontecgUserStore.FindByNameOrEmailAsync(userNameOrEmailAddress);
        }

        public virtual TUser FindByNameOrEmail(string userNameOrEmailAddress)
        {
            return KontecgUserStore.FindByNameOrEmail(userNameOrEmailAddress);
        }

        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            return KontecgUserStore.FindAllAsync(login);
        }

        public virtual List<TUser> FindAll(UserLoginInfo login)
        {
            return KontecgUserStore.FindAll(login);
        }

        public virtual Task<TUser> FindAsync(int? companyId, UserLoginInfo login)
        {
            return KontecgUserStore.FindAsync(companyId, login);
        }

        public virtual TUser Find(int? companyId, UserLoginInfo login)
        {
            return KontecgUserStore.Find(companyId, login);
        }

        public virtual Task<TUser> FindByNameOrEmailAsync(int? companyId, string userNameOrEmailAddress)
        {
            return KontecgUserStore.FindByNameOrEmailAsync(companyId, userNameOrEmailAddress);
        }

        public virtual TUser FindByNameOrEmail(int? companyId, string userNameOrEmailAddress)
        {
            return KontecgUserStore.FindByNameOrEmail(companyId, userNameOrEmailAddress);
        }

        /// <summary>
        /// Gets a user by given id.
        /// Throws exception if no user found with given id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User</returns>
        /// <exception cref="KontecgException">Throws exception if no user found with given id</exception>
        public virtual async Task<TUser> GetUserByIdAsync(long userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KontecgException("There is no user with id: " + userId);
            }

            return user;
        }

        /// <summary>
        /// Gets a user by given id.
        /// Throws exception if no user found with given id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User</returns>
        /// <exception cref="KontecgException">Throws exception if no user found with given id</exception>
        public virtual TUser GetUserById(long userId)
        {
            var user = KontecgUserStore.FindById(userId.ToString());
            if (user == null)
            {
                throw new KontecgException("There is no user with id: " + userId);
            }

            return user;
        }

        // Microsoft.AspNetCore.Identity.UserManager doesn't have required sync version for method calls in this function
        //public virtual TUser GetUserById(long userId)
        //{
        //    var user = FindById(userId.ToString());
        //    if (user == null)
        //    {
        //        throw new KontecgException("There is no user with id: " + userId);
        //    }

        //    return user;
        //}

        public override async Task<IdentityResult> UpdateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress);
            if (!result.Succeeded)
            {
                return result;
            }

            //Admin user's username can not be changed!
            if (user.UserName != KontecgUserBase.AdminUserName)
            {
                if ((await GetOldUserNameAsync(user.Id)) == KontecgUserBase.AdminUserName)
                {
                    throw new UserFriendlyException(
                        string.Format(L("CanNotRenameAdminUser"), KontecgUserBase.AdminUserName));
                }
            }

            return await base.UpdateAsync(user);
        }

        // Microsoft.AspNetCore.Identity.UserManager doesn't have required sync version for method calls in this function
        //public override IdentityResult Update(TUser user)
        //{
        //    var result = CheckDuplicateUsernameOrEmailAddress(user.Id, user.UserName, user.EmailAddress);
        //    if (!result.Succeeded)
        //    {
        //        return result;
        //    }

        //    //Admin user's username can not be changed!
        //    if (user.UserName != KontecgUserBase.AdminUserName)
        //    {
        //        if ((GetOldUserName(user.Id)) == KontecgUserBase.AdminUserName)
        //        {
        //            throw new UserFriendlyException(string.Format(L("CanNotRenameAdminUser"), KontecgUserBase.AdminUserName));
        //        }
        //    }

        //    return base.Update(user);
        //}

        public override async Task<IdentityResult> DeleteAsync(TUser user)
        {
            if (user.UserName == KontecgUserBase.AdminUserName)
            {
                throw new UserFriendlyException(string.Format(L("CanNotDeleteAdminUser"), KontecgUserBase.AdminUserName));
            }

            var result = await base.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _userLoginRepository.DeleteAsync(userLogin =>
                    userLogin.UserId == user.Id &&
                    userLogin.CompanyId == user.CompanyId
                );
            }

            return result;
        }

        // Microsoft.AspNetCore.Identity.UserManager doesn't have required sync version for method calls in this function
        //public override IdentityResult Delete(TUser user)
        //{
        //    if (user.UserName == KontecgUserBase.AdminUserName)
        //    {
        //        throw new UserFriendlyException(string.Format(L("CanNotDeleteAdminUser"), KontecgUserBase.AdminUserName));
        //    }

        //    return base.Delete(user);
        //}

        public virtual async Task<IdentityResult> ChangePasswordAsync(TUser user, string newPassword)
        {
            var errors = new List<IdentityError>();

            foreach (var validator in PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(this, user, newPassword);
                if (!validationResult.Succeeded)
                {
                    errors.AddRange(validationResult.Errors);
                }
            }

            if (errors.Any())
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            await KontecgUserStore.SetPasswordHashAsync(user, PasswordHasher.HashPassword(user, newPassword));

            await UpdateSecurityStampAsync(user);

            return IdentityResult.Success;
        }

        // IPasswordValidator doesn't have a sync version of Validate(...)
        //public virtual IdentityResult ChangePassword(TUser user, string newPassword)
        //{
        //    var errors = new List<IdentityError>();

        //    foreach (var validator in PasswordValidators)
        //    {
        //        var validationResult = validator.Validate(this, user, newPassword);
        //        if (!validationResult.Succeeded)
        //        {
        //            errors.AddRange(validationResult.Errors);
        //        }
        //    }

        //    if (errors.Any())
        //    {
        //        return IdentityResult.Failed(errors.ToArray());
        //    }

        //    KontecgUserStore.SetPasswordHash(user, PasswordHasher.HashPassword(user, newPassword));
        //    return IdentityResult.Success;
        //}

        public virtual async Task<IdentityResult> CheckDuplicateUsernameOrEmailAddressAsync(long? expectedUserId,
            string userName, string emailAddress)
        {
            var user = (await FindByNameAsync(userName));
            if (user != null && user.Id != expectedUserId)
            {
                throw new UserFriendlyException(string.Format(L("Identity.DuplicateUserName"), userName));
            }

            user = (await FindByEmailAsync(emailAddress));
            if (user != null && user.Id != expectedUserId)
            {
                throw new UserFriendlyException(string.Format(L("Identity.DuplicateEmail"), emailAddress));
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> SetRolesAsync(TUser user, string[] roleNames)
        {
            await KontecgUserStore.UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles);

            //Remove from removed roles
            foreach (var userRole in user.Roles.ToList())
            {
                var role = await RoleManager.FindByIdAsync(userRole.RoleId.ToString());
                if (role != null && roleNames.All(roleName => role.Name != roleName))
                {
                    var result = await RemoveFromRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            //Add to added roles
            foreach (var roleName in roleNames)
            {
                var role = await RoleManager.GetRoleByNameAsync(roleName);
                if (user.Roles.All(ur => ur.RoleId != role.Id))
                {
                    var result = await AddToRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(long userId, long ouId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await IsInOrganizationUnitAsync(
                    await GetUserByIdAsync(userId),
                    await _organizationUnitRepository.GetAsync(ouId)
                )
            );
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _userOrganizationUnitRepository.CountAsync(uou =>
                    uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id
                ) > 0;
            });
        }

        public virtual bool IsInOrganizationUnit(TUser user, OrganizationUnit ou)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _userOrganizationUnitRepository.Count(uou =>
                    uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id
                ) > 0;
            });
        }

        public virtual async Task AddToOrganizationUnitAsync(long userId, long ouId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await AddToOrganizationUnitAsync(
                    await GetUserByIdAsync(userId),
                    await _organizationUnitRepository.GetAsync(ouId)
                );
            });
        }

        public virtual async Task AddToOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var currentOus = await GetOrganizationUnitsAsync(user);

                if (currentOus.Any(cou => cou.Id == ou.Id))
                {
                    return;
                }

                await _userOrganizationUnitRepository.InsertAsync(new UserOrganizationUnit(user.CompanyId, user.Id,
                    ou.Id));
            });
        }

        public virtual void AddToOrganizationUnit(TUser user, OrganizationUnit ou)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var currentOus = GetOrganizationUnits(user);

                if (currentOus.Any(cou => cou.Id == ou.Id))
                {
                    return;
                }

                _userOrganizationUnitRepository.Insert(new UserOrganizationUnit(user.CompanyId, user.Id, ou.Id));
            });
        }

        public virtual async Task RemoveFromOrganizationUnitAsync(long userId, long ouId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await RemoveFromOrganizationUnitAsync(
                    await GetUserByIdAsync(userId),
                    await _organizationUnitRepository.GetAsync(ouId)
                );
            });
        }

        public virtual async Task RemoveFromOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _userOrganizationUnitRepository.DeleteAsync(uou =>
                    uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id
                );
            });
        }

        public virtual void RemoveFromOrganizationUnit(TUser user, OrganizationUnit ou)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _userOrganizationUnitRepository.Delete(
                    uou => uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id
                );
            });
        }

        public virtual async Task SetOrganizationUnitsAsync(long userId, params long[] organizationUnitIds)
        {
            await SetOrganizationUnitsAsync(
                await GetUserByIdAsync(userId),
                organizationUnitIds
            );
        }

        public virtual async Task SetOrganizationUnitsAsync(TUser user, params long[] organizationUnitIds)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (organizationUnitIds == null)
                {
                    organizationUnitIds = new long[0];
                }

                var currentOus = await GetOrganizationUnitsAsync(user);

                //Remove from removed OUs
                foreach (var currentOu in currentOus)
                {
                    if (!organizationUnitIds.Contains(currentOu.Id))
                    {
                        await RemoveFromOrganizationUnitAsync(user, currentOu);
                    }
                }

                await _unitOfWorkManager.Current.SaveChangesAsync();

                //Add to added OUs
                foreach (var organizationUnitId in organizationUnitIds)
                {
                    if (currentOus.All(ou => ou.Id != organizationUnitId))
                    {
                        await AddToOrganizationUnitAsync(
                            user,
                            await _organizationUnitRepository.GetAsync(organizationUnitId)
                        );
                    }
                }
            });
        }

        public virtual void SetOrganizationUnits(TUser user, params long[] organizationUnitIds)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (organizationUnitIds == null)
                {
                    organizationUnitIds = new long[0];
                }

                var currentOus = GetOrganizationUnits(user);

                //Remove from removed OUs
                foreach (var currentOu in currentOus)
                {
                    if (!organizationUnitIds.Contains(currentOu.Id))
                    {
                        RemoveFromOrganizationUnit(user, currentOu);
                    }
                }

                //Add to added OUs
                foreach (var organizationUnitId in organizationUnitIds)
                {
                    if (currentOus.All(ou => ou.Id != organizationUnitId))
                    {
                        AddToOrganizationUnit(
                            user,
                            _organizationUnitRepository.Get(organizationUnitId)
                        );
                    }
                }
            });
        }


        public virtual async Task<List<OrganizationUnit>> GetOrganizationUnitsAsync(TUser user)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var query = from uou in await _userOrganizationUnitRepository.GetAllAsync()
                            join ou in await _organizationUnitRepository.GetAllAsync() on uou.OrganizationUnitId equals ou.Id
                            where uou.UserId == user.Id
                            select ou;

                return await _asyncQueryableExecuter.ToListAsync(query);
            });
        }

        public virtual List<OrganizationUnit> GetOrganizationUnits(TUser user)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var query = from uou in _userOrganizationUnitRepository.GetAll()
                            join ou in _organizationUnitRepository.GetAll() on uou.OrganizationUnitId equals ou.Id
                            where uou.UserId == user.Id
                            select ou;

                return query.ToList();
            });
        }

        public virtual async Task<List<TUser>> GetUsersInOrganizationUnitAsync(
            OrganizationUnit organizationUnit,
            bool includeChildren = false)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (!includeChildren)
                {
                    var query = from uou in await _userOrganizationUnitRepository.GetAllAsync()
                                join user in Users on uou.UserId equals user.Id
                                where uou.OrganizationUnitId == organizationUnit.Id
                                select user;

                    return await _asyncQueryableExecuter.ToListAsync(query);
                }
                else
                {
                    var query = from uou in await _userOrganizationUnitRepository.GetAllAsync()
                                join user in Users on uou.UserId equals user.Id
                                join ou in await _organizationUnitRepository.GetAllAsync() on uou.OrganizationUnitId equals ou.Id
                                where ou.Code.StartsWith(organizationUnit.Code)
                                select user;

                    return await _asyncQueryableExecuter.ToListAsync(query);
                }
            });
        }

        public virtual List<TUser> GetUsersInOrganizationUnit(
            OrganizationUnit organizationUnit,
            bool includeChildren = false)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (!includeChildren)
                {
                    var query = from uou in _userOrganizationUnitRepository.GetAll()
                                join user in Users on uou.UserId equals user.Id
                                where uou.OrganizationUnitId == organizationUnit.Id
                                select user;

                    return query.ToList();
                }
                else
                {
                    var query = from uou in _userOrganizationUnitRepository.GetAll()
                                join user in Users on uou.UserId equals user.Id
                                join ou in _organizationUnitRepository.GetAll() on uou.OrganizationUnitId equals ou.Id
                                where ou.Code.StartsWith(organizationUnit.Code)
                                select user;

                    return query.ToList();
                }
            });
        }

        public virtual async Task InitializeOptionsAsync(int? companyId)
        {
            Options = JsonConvert.DeserializeObject<IdentityOptions>(JsonConvert.SerializeObject(_optionsAccessor.Value));

            //Lockout
            Options.Lockout.AllowedForNewUsers = await IsTrueAsync(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled,
                companyId
            );

            Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(
                await GetSettingValueAsync<int>(
                    KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    companyId
                )
            );

            Options.Lockout.MaxFailedAccessAttempts = await GetSettingValueAsync<int>(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                companyId
            );

            //Password complexity
            Options.Password.RequireDigit = await GetSettingValueAsync<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                companyId
            );

            Options.Password.RequireLowercase = await GetSettingValueAsync<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                companyId
            );

            Options.Password.RequireNonAlphanumeric = await GetSettingValueAsync<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                companyId
            );

            Options.Password.RequireUppercase = await GetSettingValueAsync<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                companyId
            );

            Options.Password.RequiredLength = await GetSettingValueAsync<int>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                companyId
            );
        }

        public virtual void InitializeOptions(int? companyId)
        {
            Options = JsonConvert.DeserializeObject<IdentityOptions>(JsonConvert.SerializeObject(_optionsAccessor.Value));

            //Lockout
            Options.Lockout.AllowedForNewUsers = IsTrue(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled,
                companyId
            );

            Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(
                GetSettingValue<int>(
                    KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    companyId)
            );

            Options.Lockout.MaxFailedAccessAttempts = GetSettingValue<int>(
                KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout, companyId);

            //Password complexity
            Options.Password.RequireDigit = GetSettingValue<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                companyId
            );

            Options.Password.RequireLowercase = GetSettingValue<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                companyId
            );

            Options.Password.RequireNonAlphanumeric = GetSettingValue<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                companyId
            );

            Options.Password.RequireUppercase = GetSettingValue<bool>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                companyId
            );

            Options.Password.RequiredLength = GetSettingValue<int>(
                KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                companyId
            );
        }

        protected virtual Task<string> GetOldUserNameAsync(long userId)
        {
            return KontecgUserStore.GetUserNameFromDatabaseAsync(userId);
        }

        protected virtual string GetOldUserName(long userId)
        {
            return KontecgUserStore.GetUserNameFromDatabase(userId);
        }

        private async Task<UserPermissionCacheItem> GetUserPermissionCacheItemAsync(long userId)
        {
            var cacheKey = userId + "@" + (GetCurrentCompanyId() ?? 0);
            return await _cacheManager.GetUserPermissionCache().GetAsync(cacheKey, async () =>
            {
                var user = await FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return null;
                }

                var newCacheItem = new UserPermissionCacheItem(userId);

                foreach (var roleName in await GetRolesAsync(user))
                {
                    newCacheItem.RoleIds.Add((await RoleManager.GetRoleByNameAsync(roleName)).Id);
                }

                foreach (var permissionInfo in await UserPermissionStore.GetPermissionsAsync(userId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.Add(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.ProhibitedPermissions.Add(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private UserPermissionCacheItem GetUserPermissionCacheItem(long userId)
        {
            var cacheKey = userId + "@" + (GetCurrentCompanyId() ?? 0);
            return _cacheManager.GetUserPermissionCache().Get(cacheKey, () =>
            {
                var user = KontecgUserStore.FindById(userId.ToString());
                if (user == null)
                {
                    return null;
                }

                var newCacheItem = new UserPermissionCacheItem(userId);

                foreach (var roleName in KontecgUserStore.GetRoles(user))
                {
                    newCacheItem.RoleIds.Add((RoleManager.GetRoleByName(roleName)).Id);
                }

                foreach (var permissionInfo in UserPermissionStore.GetPermissions(userId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.Add(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.ProhibitedPermissions.Add(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private bool IsTrue(string settingName, int? companyId)
        {
            return GetSettingValue<bool>(settingName, companyId);
        }

        private Task<bool> IsTrueAsync(string settingName, int? companyId)
        {
            return GetSettingValueAsync<bool>(settingName, companyId);
        }

        private T GetSettingValue<T>(string settingName, int? companyId) where T : struct
        {
            return companyId == null
                ? _settingManager.GetSettingValueForApplication<T>(settingName)
                : _settingManager.GetSettingValueForCompany<T>(settingName, companyId.Value);
        }

        private Task<T> GetSettingValueAsync<T>(string settingName, int? companyId) where T : struct
        {
            return companyId == null
                ? _settingManager.GetSettingValueForApplicationAsync<T>(settingName)
                : _settingManager.GetSettingValueForCompanyAsync<T>(settingName, companyId.Value);
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }

        private int? GetCurrentCompanyId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }

        private MultiCompanySides GetCurrentMultiCompanySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return MultiCompany.IsEnabled && !_unitOfWorkManager.Current.GetCompanyId().HasValue
                    ? MultiCompanySides.Host
                    : MultiCompanySides.Company;
            }

            return KontecgSession.MultiCompanySide;
        }

        public virtual async Task AddTokenValidityKeyAsync(
            TUser user,
            string tokenValidityKey,
            DateTime expireDate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await KontecgUserStore.AddTokenValidityKeyAsync(user, tokenValidityKey, expireDate, cancellationToken);
        }

        public virtual void AddTokenValidityKey(
            TUser user,
            string tokenValidityKey,
            DateTime expireDate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            KontecgUserStore.AddTokenValidityKey(user, tokenValidityKey, expireDate, cancellationToken);
        }

        public virtual async Task AddTokenValidityKeyAsync(
            UserIdentifier user,
            string tokenValidityKey,
            DateTime expireDate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await KontecgUserStore.AddTokenValidityKeyAsync(user, tokenValidityKey, expireDate, cancellationToken);
        }

        public virtual void AddTokenValidityKey(
            UserIdentifier user,
            string tokenValidityKey,
            DateTime expireDate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            KontecgUserStore.AddTokenValidityKey(user, tokenValidityKey, expireDate, cancellationToken);
        }

        public virtual async Task<bool> IsTokenValidityKeyValidAsync(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await KontecgUserStore.IsTokenValidityKeyValidAsync(user, tokenValidityKey, cancellationToken);
        }

        public virtual bool IsTokenValidityKeyValid(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return KontecgUserStore.IsTokenValidityKeyValid(user, tokenValidityKey, cancellationToken);
        }

        public virtual async Task RemoveTokenValidityKeyAsync(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await KontecgUserStore.RemoveTokenValidityKeyAsync(user, tokenValidityKey, cancellationToken);
        }

        public virtual void RemoveTokenValidityKey(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            KontecgUserStore.RemoveTokenValidityKey(user, tokenValidityKey, cancellationToken);
        }

        public bool IsLockedOut(string userId)
        {
            var user = KontecgUserStore.FindById(userId);
            if (user == null)
            {
                throw new KontecgException("There is no user with id: " + userId);
            }

            var lockoutEndDateUtc = KontecgUserStore.GetLockoutEndDate(user);
            return lockoutEndDateUtc > DateTimeOffset.UtcNow;
        }

        public bool IsLockedOut(TUser user)
        {
            var lockoutEndDateUtc = KontecgUserStore.GetLockoutEndDate(user);
            return lockoutEndDateUtc > DateTimeOffset.UtcNow;
        }

        public void ResetAccessFailedCount(TUser user)
        {
            KontecgUserStore.ResetAccessFailedCount(user);
        }
    }
}
