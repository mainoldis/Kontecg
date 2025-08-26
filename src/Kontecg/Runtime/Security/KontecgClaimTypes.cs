using System.Security.Claims;

namespace Kontecg.Runtime.Security
{
    /// <summary>
    ///     Used to get Kontecg-specific claim type names.
    /// </summary>
    public static class KontecgClaimTypes
    {
        /// <summary>
        ///     UserId.
        ///     Default: <see cref="ClaimTypes.Name" />
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        ///     UserId.
        ///     Default: <see cref="ClaimTypes.NameIdentifier" />
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        ///     UserId.
        ///     Default: <see cref="ClaimTypes.Role" />
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;

        /// <summary>
        ///     CompanyId.
        ///     Default: http://www.ecg.moa.minbas.cu/identity/claims/companyId
        /// </summary>
        public static string CompanyId { get; set; } = "http://www.ecg.moa.minbas.cu/identity/claims/companyId";

        /// <summary>
        ///     PersonId.
        ///     Default: http://www.ecg.moa.minbas.cu/identity/claims/personId
        /// </summary>
        public static string PersonId { get; set; } = "http://www.ecg.moa.minbas.cu/identity/claims/personId";

        /// <summary>
        ///     ImpersonatorUserId.
        ///     Default: http://www.ecg.moa.minbas.cu/identity/claims/impersonatorUserId
        /// </summary>
        public static string ImpersonatorUserId { get; set; } =
            "http://www.ecg.moa.minbas.cu/identity/claims/impersonatorUserId";

        /// <summary>
        ///     ImpersonatorCompanyId
        ///     Default: http://www.ecg.moa.minbas.cu/identity/claims/impersonatorCompanyId
        /// </summary>
        public static string ImpersonatorCompanyId { get; set; } =
            "http://www.ecg.moa.minbas.cu/identity/claims/impersonatorCompanyId";
    }
}
