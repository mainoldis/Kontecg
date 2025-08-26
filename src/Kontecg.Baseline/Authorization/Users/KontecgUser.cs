using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Represents a user.
    /// </summary>
    public abstract class KontecgUser<TUser> : KontecgUserBase, IFullAudited<TUser>
        where TUser : KontecgUser<TUser>
    {
        /// <summary>
        ///     Maximum length of the <see cref="ConcurrencyStamp" /> property.
        /// </summary>
        public const int ConcurrencyStampLength = 128;

        /// <summary>
        ///     User name.
        ///     User name must be unique for it's company.
        /// </summary>
        [Required]
        [StringLength(UserNameLength)]
        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        ///     Email address of the user.
        ///     Email address must be unique for it's company.
        /// </summary>
        [Required]
        [StringLength(EmailAddressLength)]
        public virtual string NormalizedEmailAddress { get; set; }

        /// <summary>
        ///     A random value that must change whenever a user is persisted to the store
        /// </summary>
        [StringLength(ConcurrencyStampLength)]
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<UserToken> Tokens { get; set; }

        public virtual TUser DeleterUser { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }

        public virtual void SetNormalizedNames()
        {
            NormalizedUserName = UserName.ToUpperInvariant();
            NormalizedEmailAddress = EmailAddress.ToUpperInvariant();
        }
    }
}
