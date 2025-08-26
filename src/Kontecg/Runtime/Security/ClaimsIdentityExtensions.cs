using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using JetBrains.Annotations;
using Kontecg.Extensions;

namespace Kontecg.Runtime.Security
{
    public static class ClaimsIdentityExtensions
    {
        public static UserIdentifier GetUserIdentifierOrNull(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            long? userId = identity.GetUserId();
            if (userId == null)
            {
                return null;
            }

            return new UserIdentifier(identity.GetCompanyId(), userId.Value);
        }

        public static long? GetUserId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == KontecgClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Convert.ToInt64(userIdOrNull.Value);
        }

        public static int? GetCompanyId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim companyIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == KontecgClaimTypes.CompanyId);
            if (companyIdOrNull == null || companyIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Convert.ToInt32(companyIdOrNull.Value);
        }

        public static long? GetImpersonatorUserId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim userIdOrNull =
                claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == KontecgClaimTypes.ImpersonatorUserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Convert.ToInt64(userIdOrNull.Value);
        }

        public static int? GetImpersonatorCompanyId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim companyIdOrNull =
                claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == KontecgClaimTypes.ImpersonatorCompanyId);
            if (companyIdOrNull == null || companyIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Convert.ToInt32(companyIdOrNull.Value);
        }
    }
}
