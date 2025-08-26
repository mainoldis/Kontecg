using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Organizations
{
    /// <summary>
    ///     Represents membership of a User to an OU.
    /// </summary>
    [Table("organization_unit_roles", Schema = "seg")]
    public class OrganizationUnitRole : CreationAuditedEntity<long>, IMayHaveCompany, ISoftDelete
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationUnitRole" /> class.
        /// </summary>
        public OrganizationUnitRole()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationUnitRole" /> class.
        /// </summary>
        /// <param name="companyId">CompanyId</param>
        /// <param name="roleId">Id of the User.</param>
        /// <param name="organizationUnitId">Id of the <see cref="OrganizationUnit" />.</param>
        public OrganizationUnitRole(int? companyId, int roleId, long organizationUnitId)
        {
            CompanyId = companyId;
            RoleId = roleId;
            OrganizationUnitId = organizationUnitId;
        }

        /// <summary>
        ///     Id of the Role.
        /// </summary>
        public virtual int RoleId { get; set; }

        /// <summary>
        ///     Id of the <see cref="OrganizationUnit" />.
        /// </summary>
        public virtual long OrganizationUnitId { get; set; }

        /// <summary>
        ///     CompanyId of this entity.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        /// <summary>
        ///     Specifies if the organization is soft deleted or not.
        /// </summary>
        public virtual bool IsDeleted { get; set; }
    }
}
