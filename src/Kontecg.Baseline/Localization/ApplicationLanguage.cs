using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Represents a language of the application.
    /// </summary>
    [Serializable]
    [Table("languages", Schema = "gen")]
    public class ApplicationLanguage : FullAuditedEntity, IMayHaveCompany
    {
        /// <summary>
        ///     The maximum name length.
        /// </summary>
        public const int NameLength = 128;

        /// <summary>
        ///     The maximum display name length.
        /// </summary>
        public const int DisplayNameLength = 64;

        /// <summary>
        ///     The maximum icon length.
        /// </summary>
        public const int IconLength = 128;

        /// <summary>
        ///     Creates a new <see cref="ApplicationLanguage" /> object.
        /// </summary>
        public ApplicationLanguage()
        {
        }

        public ApplicationLanguage(int? companyId, string name, string displayName, string icon = null,
            bool isDisabled = false)
        {
            CompanyId = companyId;
            Name = name;
            DisplayName = displayName;
            Icon = icon;
            IsDisabled = isDisabled;
        }

        /// <summary>
        ///     Gets or sets the name of the culture, like "en" or "en-US".
        /// </summary>
        [Required]
        [StringLength(NameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        [Required]
        [StringLength(DisplayNameLength)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        [StringLength(IconLength)]
        public virtual string Icon { get; set; }

        /// <summary>
        ///     Is this language active. Inactive languages are not get by <see cref="IApplicationLanguageManager" />.
        /// </summary>
        public virtual bool IsDisabled { get; set; }

        /// <summary>
        ///     CompanyId of this entity. Can be null for host.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        public virtual LanguageInfo ToLanguageInfo()
        {
            return new LanguageInfo(Name, DisplayName, Icon, isDisabled: IsDisabled);
        }
    }
}
