using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Represents a setting for a company or user.
    /// </summary>
    [Table("settings", Schema = "gen")]
    public class Setting : AuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int NameLength = 256;

        /// <summary>
        ///     Creates a new <see cref="Setting" /> object.
        /// </summary>
        public Setting()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="Setting" /> object.
        /// </summary>
        /// <param name="companyId">CompanyId for this setting</param>
        /// <param name="userId">UserId for this setting</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public Setting(int? companyId, long? userId, string name, string value)
        {
            CompanyId = companyId;
            UserId = userId;
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     UserId for this setting.
        ///     UserId is null if this setting is not user level.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        ///     Unique name of the setting.
        /// </summary>
        [Required]
        [StringLength(NameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Value of the setting.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        ///     CompanyId for this setting.
        ///     CompanyId is null if this setting is not Company level.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
