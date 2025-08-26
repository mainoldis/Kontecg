using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Timing;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Used to save a login attempt of a user.
    /// </summary>
    [Table("user_login_attempts", Schema = "log")]
    public class UserLoginAttempt : Entity<long>, IHasCreationTime, IMayHaveCompany
    {
        /// <summary>
        ///     Max length of the <see cref="CompanyName" /> property.
        /// </summary>
        public const int MaxCompanyNameLength = KontecgCompanyBase.MaxCompanyNameLength;

        /// <summary>
        ///     Max length of the <see cref="CompanyName" /> property.
        /// </summary>
        public const int MaxUserNameOrEmailAddressLength = KontecgUserBase.EmailAddressLength;

        /// <summary>
        ///     Maximum length of <see cref="ClientIpAddress" /> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;

        /// <summary>
        ///     Maximum length of <see cref="ClientName" /> property.
        /// </summary>
        public const int MaxClientNameLength = 128;

        /// <summary>
        ///     Maximum length of <see cref="ClientInfo" /> property.
        /// </summary>
        public const int MaxClientInfoLength = 2048;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public const int MaxFailReasonLength = 1024;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserLoginAttempt" /> class.
        /// </summary>
        public UserLoginAttempt()
        {
            CreationTime = Clock.Now;
        }

        /// <summary>
        ///     Company name.
        /// </summary>
        [StringLength(MaxCompanyNameLength)]
        public virtual string CompanyName { get; set; }

        /// <summary>
        ///     User's Id, if <see cref="UserNameOrEmailAddress" /> was a valid username or email address.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        ///     Username or email address
        /// </summary>
        [StringLength(MaxUserNameOrEmailAddressLength)]
        public virtual string UserNameOrEmailAddress { get; set; }

        /// <summary>
        ///     IP address of the client.
        /// </summary>
        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        ///     Name (generally computer name) of the client.
        /// </summary>
        [StringLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        ///     Client information if this method is called in a web request or other info about it.
        /// </summary>
        [StringLength(MaxClientInfoLength)]
        public virtual string ClientInfo { get; set; }

        /// <summary>
        ///     Login attempt result.
        /// </summary>
        public virtual KontecgLoginResultType Result { get; set; }

        public virtual DateTime CreationTime { get; set; }

        [StringLength(MaxFailReasonLength)]
        public virtual string FailReason { get; set; }

        /// <summary>
        ///     Company's Id, if <see cref="CompanyName" /> was a valid company name.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
