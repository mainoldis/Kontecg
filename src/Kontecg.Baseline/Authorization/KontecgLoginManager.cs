using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Auditing;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Configuration;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.IdentityFramework;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;

namespace Kontecg.Authorization
{
    public class KontecgLogInManager<TCompany, TRole, TUser> : ITransientDependency
    where TCompany : KontecgCompany<TUser>
    where TRole : KontecgRole<TUser>, new()
    where TUser : KontecgUser<TUser>
    {
        public IClientInfoProvider ClientInfoProvider { get; set; }

        protected IMultiCompanyConfig MultiCompanyConfig { get; }
        protected IRepository<TCompany> CompanyRepository { get; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        protected KontecgUserManager<TRole, TUser> UserManager { get; }
        protected ISettingManager SettingManager { get; }
        protected IRepository<UserLoginAttempt, long> UserLoginAttemptRepository { get; }
        protected IUserManagementConfig UserManagementConfig { get; }
        protected IIocResolver IocResolver { get; }
        protected KontecgRoleManager<TRole, TUser> RoleManager { get; }

        private readonly IPasswordHasher<TUser> _passwordHasher;

        private readonly KontecgUserClaimsPrincipalFactory<TUser, TRole> _claimsPrincipalFactory;

        public KontecgLogInManager(
            KontecgUserManager<TRole, TUser> userManager,
            IMultiCompanyConfig multiCompanyConfig,
            IRepository<TCompany> companyRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<TUser> passwordHasher,
            KontecgRoleManager<TRole, TUser> roleManager,
            KontecgUserClaimsPrincipalFactory<TUser, TRole> claimsPrincipalFactory)
        {
            _passwordHasher = passwordHasher;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            MultiCompanyConfig = multiCompanyConfig;
            CompanyRepository = companyRepository;
            UnitOfWorkManager = unitOfWorkManager;
            SettingManager = settingManager;
            UserLoginAttemptRepository = userLoginAttemptRepository;
            UserManagementConfig = userManagementConfig;
            IocResolver = iocResolver;
            RoleManager = roleManager;
            UserManager = userManager;

            ClientInfoProvider = NullClientInfoProvider.Instance;
        }

        public virtual async Task<KontecgLoginResult<TCompany, TUser>> LoginAsync(UserLoginInfo login,
            string companyName = null, bool shouldLockout = true)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var result = await LoginAsyncInternalAsync(login, companyName, shouldLockout);

                if (ShouldPreventSavingLoginAttempt(result))
                {
                    return result;
                }

                await SaveLoginAttemptAsync(result, companyName, login.ProviderKey + "@" + login.LoginProvider);
                return result;
            });
        }

        protected virtual bool ShouldPreventSavingLoginAttempt(KontecgLoginResult<TCompany, TUser> loginResult)
        {
            return loginResult.Result == KontecgLoginResultType.Success;
        }

        protected virtual async Task<KontecgLoginResult<TCompany, TUser>> LoginAsyncInternalAsync(UserLoginInfo login,
            string companyName, bool shouldLockout)
        {
            if (login == null || login.LoginProvider.IsNullOrEmpty() || login.ProviderKey.IsNullOrEmpty())
            {
                throw new ArgumentException("login");
            }

            //Get and check company
            TCompany company = null;
            if (!MultiCompanyConfig.IsEnabled)
            {
                company = await GetDefaultCompanyAsync();
            }
            else if (!string.IsNullOrWhiteSpace(companyName))
            {
                company = await CompanyRepository.FirstOrDefaultAsync(t => t.CompanyName == companyName);
                if (company == null)
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.InvalidCompanyName);
                }

                if (!company.IsActive)
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.CompanyIsNotActive, company);
                }
            }

            int? companyId = company?.Id;
            using (UnitOfWorkManager.Current.SetCompanyId(companyId))
            {
                var user = await UserManager.FindAsync(companyId, login);
                if (user == null)
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.UnknownExternalLogin, company);
                }
                if (shouldLockout)
                {
                    if (await TryLockOutAsync(companyId, user.Id))
                    {
                        return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.LockedOut, company, user);
                    }
                }

                return await CreateLoginResultAsync(user, company);
            }
        }


        public virtual async Task<KontecgLoginResult<TCompany, TUser>> LoginAsync(
            string userNameOrEmailAddress,
            string plainPassword,
            string companyName = null,
            bool shouldLockout = true)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var result = await LoginAsyncInternalAsync(
                    userNameOrEmailAddress,
                    plainPassword,
                    companyName,
                    shouldLockout
                );

                if (ShouldPreventSavingLoginAttempt(result))
                {
                    return result;
                }

                await SaveLoginAttemptAsync(result, companyName, userNameOrEmailAddress);
                return result;
            });
        }

        protected virtual async Task<KontecgLoginResult<TCompany, TUser>> LoginAsyncInternalAsync(
            string userNameOrEmailAddress,
            string plainPassword,
            string companyName,
            bool shouldLockout)
        {
            if (userNameOrEmailAddress.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userNameOrEmailAddress));
            }

            if (plainPassword.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(plainPassword));
            }

            // Get and check company
            TCompany company = null;
            using (UnitOfWorkManager.Current.SetCompanyId(null))
            {
                if (!MultiCompanyConfig.IsEnabled)
                {
                    company = await GetDefaultCompanyAsync();
                }
                else if (!string.IsNullOrWhiteSpace(companyName))
                {
                    company = await CompanyRepository.FirstOrDefaultAsync(t => t.CompanyName == companyName);
                    if (company == null)
                    {
                        return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.InvalidCompanyName);
                    }

                    if (!company.IsActive)
                    {
                        return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.CompanyIsNotActive, company);
                    }
                }
            }

            var companyId = company?.Id;
            using (UnitOfWorkManager.Current.SetCompanyId(companyId))
            {
                await UserManager.InitializeOptionsAsync(companyId);

                //TryLoginFromExternalAuthenticationSources method may create the user, that's why we are calling it before KontecgUserStore.FindByNameOrEmailAsync
                var loggedInFromExternalSource = await TryLoginFromExternalAuthenticationSourcesAsync(
                    userNameOrEmailAddress,
                    plainPassword,
                    company
                );

                var user = await UserManager.FindByNameOrEmailAsync(companyId, userNameOrEmailAddress);
                if (user == null)
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.InvalidUserNameOrEmailAddress, company);
                }

                if (await UserManager.IsLockedOutAsync(user))
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.LockedOut, company, user);
                }

                if (!loggedInFromExternalSource)
                {
                    if (!await UserManager.CheckPasswordAsync(user, plainPassword))
                    {
                        return await GetFailedPasswordValidationAsLoginResultAsync(user, company, shouldLockout);
                    }

                    await UserManager.ResetAccessFailedCountAsync(user);
                }

                return await CreateLoginResultAsync(user, company);
            }
        }

        protected virtual async Task<KontecgLoginResult<TCompany, TUser>> GetFailedPasswordValidationAsLoginResultAsync(TUser user, TCompany company = null, bool shouldLockout = false)
        {
            if (shouldLockout)
            {
                if (await TryLockOutAsync(user.CompanyId, user.Id))
                {
                    return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.LockedOut, company, user);
                }
            }

            return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.InvalidPassword, company, user);
        }

        protected virtual async Task<KontecgLoginResult<TCompany, TUser>> CreateLoginResultAsync(TUser user,
            TCompany company = null)
        {
            if (!user.IsActive)
            {
                return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.UserIsNotActive);
            }

            if (await IsEmailConfirmationRequiredForLoginAsync(user.CompanyId) && !user.IsEmailConfirmed)
            {
                return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.UserEmailIsNotConfirmed);
            }

            if (await IsPhoneConfirmationRequiredForLoginAsync(user.CompanyId) && !user.IsPhoneNumberConfirmed)
            {
                return new KontecgLoginResult<TCompany, TUser>(KontecgLoginResultType.UserPhoneNumberIsNotConfirmed);
            }

            var principal = await _claimsPrincipalFactory.CreateAsync(user);

            return new KontecgLoginResult<TCompany, TUser>(
                company,
                user,
                principal.Identity as ClaimsIdentity
            );
        }

        // Can be used after two-factor login
        public virtual async Task SaveLoginAttemptAsync(
            KontecgLoginResult<TCompany, TUser> loginResult,
            string companyName,
            string userNameOrEmailAddress)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                var companyId = loginResult.Company?.Id;
                using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var loginAttempt = new UserLoginAttempt
                    {
                        CompanyId = companyId,
                        CompanyName = companyName.TruncateWithPostfix(UserLoginAttempt.MaxCompanyNameLength),

                        UserId = loginResult.User?.Id,
                        UserNameOrEmailAddress =
                            userNameOrEmailAddress.TruncateWithPostfix(UserLoginAttempt
                                .MaxUserNameOrEmailAddressLength),

                        Result = loginResult.Result,

                        ClientInfo =
                            ClientInfoProvider.ClientInfo.TruncateWithPostfix(UserLoginAttempt.MaxClientInfoLength),
                        ClientIpAddress =
                            ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(UserLoginAttempt
                                .MaxClientIpAddressLength),
                        ClientName =
                            ClientInfoProvider.ComputerName.TruncateWithPostfix(UserLoginAttempt.MaxClientNameLength),
                    };

                    using (var localizationContext = IocResolver.ResolveAsDisposable<ILocalizationContext>())
                    {
                        loginAttempt.FailReason = loginResult
                            .GetFailReason(localizationContext.Object)
                            .TruncateWithPostfix(UserLoginAttempt.MaxFailReasonLength);
                    }

                    await UserLoginAttemptRepository.InsertAsync(loginAttempt);
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();
                }
            }
        }

        public virtual void SaveLoginAttempt(
            KontecgLoginResult<TCompany, TUser> loginResult,
            string companyName,
            string userNameOrEmailAddress)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                var companyId = loginResult.Company?.Id;
                using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var loginAttempt = new UserLoginAttempt
                    {
                        CompanyId = companyId,
                        CompanyName = companyName.TruncateWithPostfix(UserLoginAttempt.MaxCompanyNameLength),

                        UserId = loginResult.User?.Id,
                        UserNameOrEmailAddress =
                            userNameOrEmailAddress.TruncateWithPostfix(UserLoginAttempt
                                .MaxUserNameOrEmailAddressLength),

                        Result = loginResult.Result,

                        ClientInfo =
                            ClientInfoProvider.ClientInfo.TruncateWithPostfix(UserLoginAttempt.MaxClientInfoLength),
                        ClientIpAddress =
                            ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(UserLoginAttempt
                                .MaxClientIpAddressLength),
                        ClientName =
                            ClientInfoProvider.ComputerName.TruncateWithPostfix(UserLoginAttempt.MaxClientNameLength),
                    };

                    using (var localizationContext = IocResolver.ResolveAsDisposable<ILocalizationContext>())
                    {
                        loginAttempt.FailReason = loginResult
                            .GetFailReason(localizationContext.Object)
                            .TruncateWithPostfix(UserLoginAttempt.MaxFailReasonLength);
                    }

                    UserLoginAttemptRepository.Insert(loginAttempt);
                    UnitOfWorkManager.Current.SaveChanges();

                    uow.Complete();
                }
            }
        }

        protected virtual async Task<bool> TryLockOutAsync(int? companyId, long userId)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var user = await UserManager.FindByIdAsync(userId.ToString());

                    (await UserManager.AccessFailedAsync(user)).CheckErrors();

                    var isLockOut = await UserManager.IsLockedOutAsync(user);

                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();

                    return isLockOut;
                }
            }
        }

        protected virtual async Task<bool> TryLoginFromExternalAuthenticationSourcesAsync(string userNameOrEmailAddress,
            string plainPassword, TCompany company)
        {
            if (!UserManagementConfig.ExternalAuthenticationSources.Any())
            {
                return false;
            }

            foreach (var sourceType in UserManagementConfig.ExternalAuthenticationSources)
            {
                using (var source =
                       IocResolver.ResolveAsDisposable<IExternalAuthenticationSource<TCompany, TUser>>(sourceType))
                {
                    if (await source.Object.TryAuthenticateAsync(userNameOrEmailAddress, plainPassword, company))
                    {
                        var companyId = company?.Id;
                        using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                        {
                            var user = await UserManager.FindByNameOrEmailAsync(companyId, userNameOrEmailAddress);
                            if (user == null)
                            {
                                user = await source.Object.CreateUserAsync(userNameOrEmailAddress, company);

                                user.CompanyId = companyId;
                                user.AuthenticationSource = source.Object.Name;
                                user.Password =
                                    _passwordHasher.HashPassword(user,
                                        Guid.NewGuid().ToString("N")
                                            .Left(16)); //Setting a random password since it will not be used
                                user.SetNormalizedNames();

                                if (user.Roles == null)
                                {
                                    user.Roles = new List<UserRole>();
                                    foreach (var defaultRole in RoleManager.Roles
                                                 .Where(r => r.CompanyId == companyId && r.IsDefault).ToList())
                                    {
                                        user.Roles.Add(new UserRole(companyId, user.Id, defaultRole.Id));
                                    }
                                }

                                await UserManager.CreateAsync(user);
                            }
                            else
                            {
                                await source.Object.UpdateUserAsync(user, company);

                                user.AuthenticationSource = source.Object.Name;

                                await UserManager.UpdateAsync(user);
                            }

                            await UnitOfWorkManager.Current.SaveChangesAsync();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected virtual async Task<TCompany> GetDefaultCompanyAsync()
        {
            var company = await CompanyRepository.FirstOrDefaultAsync(
                t => t.CompanyName == KontecgCompanyBase.DefaultCompanyName
            );
            if (company == null)
            {
                throw new KontecgException("There should be a 'Default' company if multi-company is disabled!");
            }

            return company;
        }

        protected virtual async Task<bool> IsEmailConfirmationRequiredForLoginAsync(int? companyId)
        {
            if (companyId.HasValue)
            {
                return await SettingManager.GetSettingValueForCompanyAsync<bool>(
                    KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                    companyId.Value
                );
            }

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(
                KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin
            );
        }

        protected virtual Task<bool> IsPhoneConfirmationRequiredForLoginAsync(int? companyId)
        {
            return Task.FromResult(false);
        }
    }
}
