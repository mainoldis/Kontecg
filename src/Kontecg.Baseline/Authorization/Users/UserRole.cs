using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Represents role record of a user.
    /// </summary>
    [Table("user_roles", Schema = "seg")]
    public class UserRole : CreationAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Creates a new <see cref="UserRole" /> object.
        /// </summary>
        public UserRole()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="UserRole" /> object.
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <param name="userId">User id</param>
        /// <param name="roleId">Role id</param>
        public UserRole(int? companyId, long userId, int roleId)
        {
            CompanyId = companyId;
            UserId = userId;
            RoleId = roleId;
        }

        /// <summary>
        ///     User id.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        ///     Role id.
        /// </summary>
        public virtual int RoleId { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
