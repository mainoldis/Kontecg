#if false


using System.Threading.Tasks;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kontecg.Authorization
{
    public class KontecgSecurityStampValidator<TCompany, TRole, TUser> : SecurityStampValidator<TUser>
        where TCompany : KontecgCompany<TUser>
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public KontecgSecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            KontecgSignInManager<TCompany, TRole, TUser> signInManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                options,
                signInManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => { await base.ValidateAsync(context); });
        }
    }
}
#endif
