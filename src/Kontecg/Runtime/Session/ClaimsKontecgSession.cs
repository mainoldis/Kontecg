using System;
using System.Linq;
using System.Security.Claims;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Security;

namespace Kontecg.Runtime.Session
{
    /// <summary>
    ///     Implements <see cref="IKontecgSession" /> to get session properties from current claims.
    /// </summary>
    public class ClaimsKontecgSession : KontecgSessionBase, ISingletonDependency
    {
        public ClaimsKontecgSession(
            IPrincipalAccessor principalAccessor,
            IMultiCompanyConfig multiCompany,
            ICompanyResolver companyResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
            : base(
                multiCompany,
                sessionOverrideScopeProvider)
        {
            CompanyResolver = companyResolver;
            PrincipalAccessor = principalAccessor;
        }

        public override long? UserId
        {
            get
            {
                if (OverridedValue != null)
                {
                    return OverridedValue.UserId;
                }

                Claim userIdClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KontecgClaimTypes.UserId);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                if (!long.TryParse(userIdClaim.Value, out long userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public override int? CompanyId
        {
            get
            {
                if (!MultiCompany.IsEnabled)
                {
                    return MultiCompanyConsts.DefaultCompanyId;
                }

                if (OverridedValue != null)
                {
                    return OverridedValue.CompanyId;
                }

                Claim companyIdClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KontecgClaimTypes.CompanyId);
                if (!string.IsNullOrEmpty(companyIdClaim?.Value))
                {
                    return Convert.ToInt32(companyIdClaim.Value);
                }

                if (UserId == null)
                    //Resolve company id from resolvers only if user has not logged in!
                {
                    return CompanyResolver.ResolveCompanyId();
                }

                return null;
            }
        }

        /// <summary>
        ///     Impersonator UserId for a user
        /// </summary>
        public override long? ImpersonatorUserId
        {
            get
            {
                Claim impersonatorUserIdClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(c =>
                        c.Type == KontecgClaimTypes.ImpersonatorUserId);
                if (string.IsNullOrEmpty(impersonatorUserIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt64(impersonatorUserIdClaim.Value);
            }
        }

        public override int? ImpersonatorCompanyId
        {
            get
            {
                if (!MultiCompany.IsEnabled)
                {
                    return MultiCompanyConsts.DefaultCompanyId;
                }

                Claim impersonatorCompanyIdClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(c =>
                        c.Type == KontecgClaimTypes.ImpersonatorCompanyId);
                if (string.IsNullOrEmpty(impersonatorCompanyIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt32(impersonatorCompanyIdClaim.Value);
            }
        }

        protected IPrincipalAccessor PrincipalAccessor { get; }

        protected ICompanyResolver CompanyResolver { get; }
    }
}
