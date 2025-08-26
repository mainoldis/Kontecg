using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.DynamicEntityProperties
{
    [Table("dynamic_property_values", Schema = "gen")]
    public class DynamicPropertyValue : Entity<long>, IMayHaveCompany
    {
        public DynamicPropertyValue()
        {
        }

        public DynamicPropertyValue(DynamicProperty dynamicProperty, string value, int? companyId)
        {
            Value = value;
            CompanyId = companyId;
            DynamicPropertyId = dynamicProperty.Id;
        }

        /// <summary>
        ///     Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public virtual string Value { get; set; }

        public virtual int DynamicPropertyId { get; set; }

        [ForeignKey("DynamicPropertyId")] public virtual DynamicProperty DynamicProperty { get; set; }

        public virtual int? CompanyId { get; set; }
    }
}
