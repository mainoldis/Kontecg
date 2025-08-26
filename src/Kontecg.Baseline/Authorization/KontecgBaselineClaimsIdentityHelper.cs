using System;
using System.Security.Claims;
using Kontecg.Runtime.Security;

namespace Kontecg.Authorization
{
    internal static class KontecgBaselineClaimsIdentityHelper
    {
        public static int? GetCompanyId(ClaimsPrincipal principal)
        {
            string companyIdOrNull = principal?.FindFirstValue(KontecgClaimTypes.CompanyId);
            if (companyIdOrNull == null)
            {
                return null;
            }

            return Convert.ToInt32(companyIdOrNull);
        }
    }
}
