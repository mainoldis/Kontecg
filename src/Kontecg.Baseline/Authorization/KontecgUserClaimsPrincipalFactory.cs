using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Security;
using Kontecg.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kontecg.Authorization
{
    public class KontecgUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>,
        ITransientDependency
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public KontecgUserClaimsPrincipalFactory(
            KontecgUserManager<TRole, TUser> userManager,
            KontecgRoleManager<TRole, TUser> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IUnitOfWorkManager unitOfWorkManager)
            : base(userManager, roleManager, optionsAccessor)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                ClaimsPrincipal principal = await base.CreateAsync(user);

                if (user.CompanyId.HasValue)
                {
                    principal.Identities.First()
                        .AddClaim(new Claim(KontecgClaimTypes.CompanyId, user.CompanyId.ToString()));
                }

                return principal;
            });
        }

        public virtual ClaimsPrincipal Create(TUser user)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                ClaimsPrincipal principal = AsyncHelper.RunSync(() => base.CreateAsync(user));

                if (user.CompanyId.HasValue)
                {
                    principal.Identities.First()
                        .AddClaim(new Claim(KontecgClaimTypes.CompanyId, user.CompanyId.ToString()));
                }

                return principal;
            });
        }
    }
}
