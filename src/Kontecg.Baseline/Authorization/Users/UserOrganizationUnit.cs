using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Organizations;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Represents membership of a User to an OU.
    /// </summary>
    [Table("user_organization_units", Schema = "seg")]
    public class UserOrganizationUnit : CreationAuditedEntity<long>, IMayHaveCompany, ISoftDelete
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserOrganizationUnit" /> class.
        /// </summary>
        public UserOrganizationUnit()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserOrganizationUnit" /> class.
        /// </summary>
        /// <param name="companyId">CompanyId</param>
        /// <param name="userId">Id of the User.</param>
        /// <param name="organizationUnitId">Id of the <see cref="OrganizationUnit" />.</param>
        public UserOrganizationUnit(int? companyId, long userId, long organizationUnitId)
        {
            CompanyId = companyId;
            UserId = userId;
            OrganizationUnitId = organizationUnitId;
        }

        /// <summary>
        ///     Id of the User.
        /// </summary>
        public virtual long UserId { get; set; }

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
