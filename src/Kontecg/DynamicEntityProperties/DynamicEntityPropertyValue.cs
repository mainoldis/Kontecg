using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.DynamicEntityProperties
{
    [Table("dynamic_entity_property_values", Schema = "gen")]
    public class DynamicEntityPropertyValue : Entity<long>, IMayHaveCompany
    {
        public DynamicEntityPropertyValue()
        {
        }

        public DynamicEntityPropertyValue(DynamicEntityProperty dynamicEntityProperty, string entityId, string value,
            int? companyId)
        {
            DynamicEntityPropertyId = dynamicEntityProperty.Id;
            EntityId = entityId;
            Value = value;
            CompanyId = companyId;
        }

        [Required(AllowEmptyStrings = false)] public string Value { get; set; }

        public string EntityId { get; set; }

        public int DynamicEntityPropertyId { get; set; }

        public virtual DynamicEntityProperty DynamicEntityProperty { get; set; }

        public int? CompanyId { get; set; }
    }
}
