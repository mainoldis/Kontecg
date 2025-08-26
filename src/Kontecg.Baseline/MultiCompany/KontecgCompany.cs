using Kontecg.Authorization.Users;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Represents a Company of the application.
    /// </summary>
    public abstract class KontecgCompany<TUser> : KontecgCompanyBase, IFullAudited<TUser>
        where TUser : KontecgUserBase
    {
        /// <summary>
        ///     Creates a new company.
        /// </summary>
        protected KontecgCompany()
        {
            IsActive = true;
        }

        /// <summary>
        ///     Creates a new company.
        /// </summary>
        /// <param name="companyName">UNIQUE name of this Company</param>
        /// <param name="name">Display name of the Company</param>
        protected KontecgCompany(string companyName, string name)
            : this()
        {
            CompanyName = companyName;
            Name = name;
        }

        /// <summary>
        ///     Reference to the creator user of this entity.
        /// </summary>
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        ///     Reference to the last modifier user of this entity.
        /// </summary>
        public virtual TUser LastModifierUser { get; set; }

        /// <summary>
        ///     Reference to the deleter user of this entity.
        /// </summary>
        public virtual TUser DeleterUser { get; set; }
    }
}
