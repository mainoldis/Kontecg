using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Roles
{
    /// <summary>
    ///     Represents a role in an application. A role is used to group permissions.
    /// </summary>
    /// <remarks>
    ///     Application should use permissions to check if user is granted to perform an operation.
    ///     Checking 'if a user has a role' is not possible until the role is static (<see cref="KontecgRoleBase.IsStatic" />).
    ///     Static roles can be used in the code and can not be deleted by users.
    ///     Non-static (dynamic) roles can be added/removed by users and we can not know their name while coding.
    ///     A user can have multiple roles. Thus, user will have all permissions of all assigned roles.
    /// </remarks>
    public abstract class KontecgRole<TUser> : KontecgRoleBase, IFullAudited<TUser>
        where TUser : KontecgUser<TUser>
    {
        /// <summary>
        ///     Maximum length of the <see cref="ConcurrencyStamp" /> property.
        /// </summary>
        public const int MaxConcurrencyStampLength = 128;

        protected KontecgRole()
        {
            SetNormalizedName();
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgRole{TUser}" /> object.
        /// </summary>
        /// <param name="companyId">CompanyId or null (if this is not a company-level role)</param>
        /// <param name="displayName">Display name of the role</param>
        protected KontecgRole(int? companyId, string displayName)
            : base(companyId, displayName)
        {
            SetNormalizedName();
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgRole{TUser}" /> object.
        /// </summary>
        /// <param name="companyId">CompanyId or null (if this is not a company-level role)</param>
        /// <param name="name">Unique role name</param>
        /// <param name="displayName">Display name of the role</param>
        protected KontecgRole(int? companyId, string name, string displayName)
            : base(companyId, name, displayName)
        {
            SetNormalizedName();
        }

        /// <summary>
        ///     Unique name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string NormalizedName { get; set; }

        /// <summary>
        ///     Claims of this user.
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual ICollection<RoleClaim> Claims { get; set; }

        /// <summary>
        ///     A random value that must change whenever a user is persisted to the store
        /// </summary>
        [StringLength(MaxConcurrencyStampLength)]
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public virtual TUser DeleterUser { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }

        public virtual void SetNormalizedName()
        {
            NormalizedName = Name.ToUpperInvariant();
        }
    }
}
