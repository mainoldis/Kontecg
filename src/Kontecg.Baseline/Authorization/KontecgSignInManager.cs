#if false

using System.Security.Claims;
using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kontecg.Authorization
{
    public class KontecgSignInManager<TCompany, TRole, TUser> : SignInManager<TUser>, ITransientDependency
        where TCompany : KontecgCompany<TUser>
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public KontecgSignInManager(
            KontecgUserManager<TRole, TUser> userManager,
            IHttpContextAccessor contextAccessor,
            KontecgUserClaimsPrincipalFactory<TUser, TRole> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes
        )
            : base(
                userManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                schemes
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
        }

        public virtual async Task SignOutAndSignInAsync(ClaimsIdentity identity, bool isPersistent)
        {
            await SignOutAsync();
            await SignInAsync(identity, isPersistent);
        }

        public virtual async Task SignInAsync(ClaimsIdentity identity, bool isPersistent)
        {
            await Context.SignInAsync(IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties {IsPersistent = isPersistent}
            );
        }

        public override async Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await base.SignInAsync(user, isPersistent, authenticationMethod);
            });
        }

        public async Task<int?> GetVerifiedCompanyIdAsync()
        {
            AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (result?.Principal == null)
            {
                return null;
            }

            return KontecgBaselineClaimsIdentityHelper.GetCompanyId(result.Principal);
        }

        private bool IsTrue(string settingName, int? companyId)
        {
            return companyId == null
                ? _settingManager.GetSettingValueForApplication<bool>(settingName)
                : _settingManager.GetSettingValueForCompany<bool>(settingName, companyId.Value);
        }
    }
}
#endif
