using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Used to store a localization text.
    /// </summary>
    [Serializable]
    [Table("language_texts", Schema = "gen")]
    public class ApplicationLanguageText : AuditedEntity<long>, IMayHaveCompany
    {
        public const int SourceNameLength = 128;
        public const int KeyLength = 256;
        public const int ValueLength = 64 * 1024 * 1024; //64KB

        /// <summary>
        ///     Language name (culture name). Matches to <see cref="ApplicationLanguage.Name" />.
        /// </summary>
        [Required]
        [StringLength(ApplicationLanguage.NameLength)]
        public virtual string LanguageName { get; set; }

        /// <summary>
        ///     Localization source name
        /// </summary>
        [Required]
        [StringLength(SourceNameLength)]
        public virtual string Source { get; set; }

        /// <summary>
        ///     Localization key
        /// </summary>
        [Required]
        [StringLength(KeyLength)]
        public virtual string Key { get; set; }

        /// <summary>
        ///     Localized value
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(ValueLength)]
        public virtual string Value { get; set; }

        /// <summary>
        ///     CompanyId of this entity. Can be null for host.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
