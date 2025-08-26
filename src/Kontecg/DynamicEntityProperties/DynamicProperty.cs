using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.DynamicEntityProperties
{
    [Table("dynamic_properties", Schema = "gen")]
    public class DynamicProperty : Entity, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="PropertyName" /> property.
        /// </summary>
        public const int PropertyNameLength = 256;

        [StringLength(PropertyNameLength)] public string PropertyName { get; set; }

        public string DisplayName { get; set; }

        public string InputType { get; set; }

        public string Permission { get; set; }

        public virtual ICollection<DynamicPropertyValue> DynamicPropertyValues { get; set; }

        public int? CompanyId { get; set; }
    }
}
