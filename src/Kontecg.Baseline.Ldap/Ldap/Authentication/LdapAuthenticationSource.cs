using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Ldap.Configuration;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.Ldap.Authentication
{
    /// <summary>
    ///     Implements <see cref="IExternalAuthenticationSource{TCompany,TUser}" /> to authenticate users from LDAP.
    ///     Extend this class using application's User and Company classes as type parameters.
    ///     Also, all needed methods can be overridden and changed upon your needs.
    /// </summary>
    /// <typeparam name="TCompany">Company type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class LdapAuthenticationSource<TCompany, TUser> :
        DefaultExternalAuthenticationSource<TCompany, TUser>,
        ITransientDependency
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase, new()
    {
        /// <summary>
        ///     LDAP
        /// </summary>
        public const string SourceName = "LDAP";

        private readonly IKontecgLdapModuleConfig _ldapModuleConfig;

        private readonly ILdapSettings _settings;

        protected LdapAuthenticationSource(ILdapSettings settings, IKontecgLdapModuleConfig ldapModuleConfig)
        {
            _settings = settings;
            _ldapModuleConfig = ldapModuleConfig;
        }

        public override string Name => SourceName;

        /// <inheritdoc />
        public override async Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword,
            TCompany Company)
        {
            if (!_ldapModuleConfig.IsEnabled || !await _settings.GetIsEnabledAsync(Company?.Id))
            {
                return false;
            }

            using (PrincipalContext principalContext =
                   await CreatePrincipalContextAsync(Company, userNameOrEmailAddress))
            {
                return ValidateCredentials(principalContext, userNameOrEmailAddress, plainPassword);
            }
        }

        /// <inheritdoc />
        public override async Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TCompany company)
        {
            await CheckIsEnabledAsync(company);

            TUser user = await base.CreateUserAsync(userNameOrEmailAddress, company);

            using (PrincipalContext principalContext = await CreatePrincipalContextAsync(company, user))
            {
                UserPrincipal userPrincipal = FindUserPrincipalByIdentity(principalContext, userNameOrEmailAddress);

                if (userPrincipal == null)
                {
                    throw new KontecgException("Unknown LDAP user: " + userNameOrEmailAddress);
                }

                UpdateUserFromPrincipal(user, userPrincipal);

                user.IsEmailConfirmed = true;
                user.IsActive = true;

                return user;
            }
        }

        public override async Task UpdateUserAsync(TUser user, TCompany Company)
        {
            await CheckIsEnabledAsync(Company);

            await base.UpdateUserAsync(user, Company);

            using (PrincipalContext principalContext = await CreatePrincipalContextAsync(Company, user))
            {
                UserPrincipal userPrincipal = FindUserPrincipalByIdentity(principalContext, user.UserName);

                if (userPrincipal == null)
                {
                    throw new KontecgException("Unknown LDAP user: " + user.UserName);
                }

                UpdateUserFromPrincipal(user, userPrincipal);
            }
        }

        protected virtual bool ValidateCredentials(PrincipalContext principalContext, string userNameOrEmailAddress,
            string plainPassword)
        {
            return principalContext.ValidateCredentials(userNameOrEmailAddress, plainPassword,
                ContextOptions.Negotiate);
        }

        protected virtual void UpdateUserFromPrincipal(TUser user, UserPrincipal userPrincipal)
        {
            user.UserName = GetUsernameFromUserPrincipal(userPrincipal);

            user.Name = userPrincipal.GivenName;
            user.Surname = userPrincipal.Surname;
            user.EmailAddress = userPrincipal.EmailAddress;

            if (userPrincipal.Enabled.HasValue)
            {
                user.IsActive = userPrincipal.Enabled.Value;
            }
        }

        protected virtual UserPrincipal FindUserPrincipalByIdentity(
            PrincipalContext principalContext,
            string userNameOrEmailAddress)
        {
            UserPrincipal userPrincipal =
                UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userNameOrEmailAddress) ??
                UserPrincipal.FindByIdentity(principalContext, IdentityType.UserPrincipalName, userNameOrEmailAddress);

            return userPrincipal;
        }

        protected virtual string GetUsernameFromUserPrincipal(UserPrincipal userPrincipal)
        {
            return userPrincipal.SamAccountName.IsNullOrEmpty() || _ldapModuleConfig.UseUserPrincipalNameAsUserName
                ? userPrincipal.UserPrincipalName
                : userPrincipal.SamAccountName;
        }

        protected virtual Task<PrincipalContext> CreatePrincipalContextAsync(TCompany company,
            string userNameOrEmailAddress)
        {
            return CreatePrincipalContextAsync(company);
        }

        protected virtual Task<PrincipalContext> CreatePrincipalContextAsync(TCompany company, TUser user)
        {
            return CreatePrincipalContextAsync(company);
        }

        protected virtual async Task<PrincipalContext> CreatePrincipalContextAsync(TCompany company)
        {
            bool useSsl = await _settings.GetUseSslAsync(company?.Id);
            ContextType contextType = await _settings.GetContextTypeAsync(company?.Id);

            ContextOptions options = useSsl
                ? ContextOptions.SecureSocketLayer | ContextOptions.Negotiate
                : GetDefaultOptionForStore(contextType);

            return new PrincipalContext(
                contextType,
                ConvertToNullIfEmpty(await _settings.GetDomainAsync(company?.Id)),
                ConvertToNullIfEmpty(await _settings.GetContainerAsync(company?.Id)),
                options,
                ConvertToNullIfEmpty(await _settings.GetUserNameAsync(company?.Id)),
                ConvertToNullIfEmpty(await _settings.GetPasswordAsync(company?.Id))
            );
        }

        private ContextOptions GetDefaultOptionForStore(ContextType contextType)
        {
            if (contextType == ContextType.Machine)
            {
                return ContextOptions.Negotiate;
            }

            return ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing;
        }

        protected virtual async Task CheckIsEnabledAsync(TCompany Company)
        {
            if (!_ldapModuleConfig.IsEnabled)
            {
                throw new KontecgException("Ldap Authentication module is disabled globally!");
            }

            int? companyId = Company?.Id;
            if (!await _settings.GetIsEnabledAsync(companyId))
            {
                throw new KontecgException("Ldap Authentication is disabled for given Company (id:" + companyId +
                                           ")! You can enable it by setting '" + LdapSettingNames.IsEnabled +
                                           "' to true");
            }
        }

        protected static string ConvertToNullIfEmpty(string str)
        {
            return str.IsNullOrWhiteSpace()
                ? null
                : str;
        }
    }
}
