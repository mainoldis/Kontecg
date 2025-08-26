using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Used to store a User Login for external Login services.
    /// </summary>
    [Table("user_logins", Schema = "seg")]
    public class UserLogin : Entity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of <see cref="LoginProvider" /> property.
        /// </summary>
        public const int MaxLoginProviderLength = 128;

        /// <summary>
        ///     Maximum length of <see cref="ProviderKey" /> property.
        /// </summary>
        public const int MaxProviderKeyLength = 256;

        public UserLogin()
        {
        }

        public UserLogin(int? companyId, long userId, string loginProvider, string providerKey)
        {
            CompanyId = companyId;
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        /// <summary>
        ///     Id of the User.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        ///     Login Provider.
        /// </summary>
        [Required]
        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        /// <summary>
        ///     Key in the <see cref="LoginProvider" />.
        /// </summary>
        [Required]
        [StringLength(MaxProviderKeyLength)]
        public virtual string ProviderKey { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
