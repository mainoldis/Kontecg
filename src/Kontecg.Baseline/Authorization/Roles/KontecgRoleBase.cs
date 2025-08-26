using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Roles
{
    /// <summary>
    ///     Base class for role.
    /// </summary>
    [Table("roles", Schema = "seg")]
    public abstract class KontecgRoleBase : FullAuditedEntity<int>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="DisplayName" /> property.
        /// </summary>
        public const int MaxDisplayNameLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 32;

        protected KontecgRoleBase()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        protected KontecgRoleBase(int? companyId, string displayName)
            : this()
        {
            CompanyId = companyId;
            DisplayName = displayName;
        }

        protected KontecgRoleBase(int? companyId, string name, string displayName)
            : this(companyId, displayName)
        {
            Name = name;
        }

        /// <summary>
        ///     Unique name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Display name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        ///     Is this a static role?
        ///     Static roles can not be deleted, can not change their name.
        ///     They can be used programmatically.
        /// </summary>
        public virtual bool IsStatic { get; set; }

        /// <summary>
        ///     Is this role will be assigned to new users as default?
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        ///     List of permissions of the role.
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual ICollection<RolePermissionSetting> Permissions { get; set; }

        /// <summary>
        ///     Company's Id, if this role is a company-level role. Null, if not.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        public override string ToString()
        {
            return $"[Role {Id}, Name={Name}]";
        }
    }
}
