using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Base class for feature settings
    /// </summary>
    [Table("features", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public abstract class FeatureSetting : CreationAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="Name" /> field.
        /// </summary>
        public const int NameLength = 128;

        /// <summary>
        ///     Maximum length of the <see cref="Value" /> property.
        /// </summary>
        public const int ValueLength = 2000;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureSetting" /> class.
        /// </summary>
        protected FeatureSetting()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureSetting" /> class.
        /// </summary>
        /// <param name="name">Feature name.</param>
        /// <param name="value">Feature value.</param>
        protected FeatureSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Feature name.
        /// </summary>
        [Required]
        [StringLength(NameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Value.
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(ValueLength)]
        public virtual string Value { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
