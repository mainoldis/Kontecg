using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Configuration;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Base class for user.
    /// </summary>
    [Table("users", Schema = "seg")]
    public abstract class KontecgUserBase : FullAuditedEntity<long>, IMayHaveCompany, IPassivable
    {
        /// <summary>
        ///     Maximum length of the <see cref="UserName" /> property.
        /// </summary>
        public const int UserNameLength = 256;

        /// <summary>
        ///     Maximum length of the <see cref="EmailAddress" /> property.
        /// </summary>
        public const int EmailAddressLength = 256;

        /// <summary>
        ///     Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int NameLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="Surname" /> property.
        /// </summary>
        public const int SurnameLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="AuthenticationSource" /> property.
        /// </summary>
        public const int AuthenticationSourceLength = 64;

        /// <summary>
        ///     UserName of the admin.
        ///     admin can not be deleted and UserName of the admin can not be changed.
        /// </summary>
        public const string AdminUserName = "kontecg";

        /// <summary>
        ///     Maximum length of the <see cref="Password" /> property.
        /// </summary>
        public const int PasswordLength = 128;

        /// <summary>
        ///     Maximum length of the <see cref="Password" /> without hashed.
        /// </summary>
        public const int PlainPasswordLength = 32;

        /// <summary>
        ///     Maximum length of the <see cref="EmailConfirmationCode" /> property.
        /// </summary>
        public const int EmailConfirmationCodeLength = 328;

        /// <summary>
        ///     Maximum length of the <see cref="PasswordResetCode" /> property.
        /// </summary>
        public const int PasswordResetCodeLength = 328;

        /// <summary>
        ///     Maximum length of the <see cref="PhoneNumber" /> property.
        /// </summary>
        public const int PhoneNumberLength = 32;

        /// <summary>
        ///     Maximum length of the <see cref="SecurityStamp" /> property.
        /// </summary>
        public const int SecurityStampLength = 128;

        protected KontecgUserBase()
        {
            IsActive = true;
            SecurityStamp = UuidGenerator.Instance.Create().ToString();
        }

        /// <summary>
        ///     Authorization source name.
        ///     It's set to external authentication source name if created by an external source.
        ///     Default: null.
        /// </summary>
        [StringLength(AuthenticationSourceLength)]
        public virtual string AuthenticationSource { get; set; }

        /// <summary>
        ///     User name.
        ///     User name must be unique for it's company.
        /// </summary>
        [Required]
        [StringLength(UserNameLength)]
        public virtual string UserName { get; set; }

        /// <summary>
        ///     Email address of the user.
        ///     Email address must be unique for it's company.
        /// </summary>
        [Required]
        [StringLength(EmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        ///     Name of the user.
        /// </summary>
        [Required]
        [StringLength(NameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Surname of the user.
        /// </summary>
        [Required]
        [StringLength(SurnameLength)]
        public virtual string Surname { get; set; }

        /// <summary>
        ///     Return full name (Name Surname )
        /// </summary>
        [NotMapped]
        public virtual string FullName => Name + " " + Surname;

        /// <summary>
        ///     Password of the user.
        /// </summary>
        [Required]
        [StringLength(PasswordLength)]
        public virtual string Password { get; set; }

        /// <summary>
        ///     Confirmation code for email.
        /// </summary>
        [StringLength(EmailConfirmationCodeLength)]
        public virtual string EmailConfirmationCode { get; set; }

        /// <summary>
        ///     Reset code for password.
        ///     It's not valid if it's null.
        ///     It's for one usage and must be set to null after reset.
        /// </summary>
        [StringLength(PasswordResetCodeLength)]
        public virtual string PasswordResetCode { get; set; }

        /// <summary>
        ///     Lockout end date.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        ///     Gets or sets the access failed count.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        ///     Gets or sets the lockout enabled.
        /// </summary>
        public virtual bool IsLockoutEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the phone number.
        /// </summary>
        [StringLength(PhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        ///     Is the <see cref="PhoneNumber" /> confirmed.
        /// </summary>
        public virtual bool IsPhoneNumberConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the security stamp.
        /// </summary>
        [StringLength(SecurityStampLength)]
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        ///     Login definitions for this user.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserLogin> Logins { get; set; }

        /// <summary>
        ///     Roles of this user.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserRole> Roles { get; set; }

        /// <summary>
        ///     Claims of this user.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserClaim> Claims { get; set; }

        /// <summary>
        ///     Permission definitions for this user.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserPermissionSetting> Permissions { get; set; }

        /// <summary>
        ///     Settings for this user.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<Setting> Settings { get; set; }

        /// <summary>
        ///     Is the <see cref="KontecgUserBase.EmailAddress" /> confirmed.
        /// </summary>
        public virtual bool IsEmailConfirmed { get; set; }

        /// <summary>
        ///     Company Id of this user.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        /// <summary>
        ///     Is this user active?
        ///     If as user is not active, he/she can not use the application.
        /// </summary>
        public virtual bool IsActive { get; set; }

        public virtual void SetNewPasswordResetCode()
        {
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(PasswordResetCodeLength);
        }

        public virtual void SetNewEmailConfirmationCode()
        {
            EmailConfirmationCode = Guid.NewGuid().ToString("N").Truncate(EmailConfirmationCodeLength);
        }

        /// <summary>
        ///     Creates <see cref="UserIdentifier" /> from this User.
        /// </summary>
        /// <returns></returns>
        public virtual UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(CompanyId, Id);
        }

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}
