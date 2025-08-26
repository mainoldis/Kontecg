using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.DynamicEntityProperties
{
    [Table("dynamic_entity_properties", Schema = "gen")]
    public class DynamicEntityProperty : Entity, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="EntityFullName" /> property.
        /// </summary>
        public const int EntityFullNameLength = 256;

        [StringLength(EntityFullNameLength)] public string EntityFullName { get; set; }

        [Required] public int DynamicPropertyId { get; set; }

        [ForeignKey("DynamicPropertyId")] public virtual DynamicProperty DynamicProperty { get; set; }

        public int? CompanyId { get; set; }
    }
}
