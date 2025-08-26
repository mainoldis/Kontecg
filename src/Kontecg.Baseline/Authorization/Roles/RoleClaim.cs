using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Roles
{
    [Table("role_claims", Schema = "seg")]
    public class RoleClaim : CreationAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="ClaimType" /> property.
        /// </summary>
        public const int MaxClaimTypeLength = 256;

        public RoleClaim()
        {
        }

        public RoleClaim(KontecgRoleBase role, Claim claim)
        {
            CompanyId = role.CompanyId;
            RoleId = role.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public virtual int RoleId { get; set; }

        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
