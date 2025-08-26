using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Users
{
    [Table("user_claims", Schema = "seg")]
    public class UserClaim : CreationAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="ClaimType" /> property.
        /// </summary>
        public const int ClaimTypeLength = 256;

        public UserClaim()
        {
        }

        public UserClaim(KontecgUserBase user, Claim claim)
        {
            CompanyId = user.CompanyId;
            UserId = user.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public virtual long UserId { get; set; }

        [StringLength(ClaimTypeLength)] public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
