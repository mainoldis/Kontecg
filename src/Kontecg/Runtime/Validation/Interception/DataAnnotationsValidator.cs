using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kontecg.Collections.Extensions;

namespace Kontecg.Runtime.Validation.Interception
{
    public class DataAnnotationsValidator : IMethodParameterValidator
    {
        public virtual IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            return GetDataAnnotationAttributeErrors(validatingObject);
        }

        /// <summary>
        ///     Checks all properties for DataAnnotations attributes.
        /// </summary>
        protected virtual List<ValidationResult> GetDataAnnotationAttributeErrors(object validatingObject)
        {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            IEnumerable<PropertyDescriptor> properties =
                TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (PropertyDescriptor property in properties)
            {
                ValidationAttribute[] validationAttributes =
                    property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                ValidationContext validationContext = new ValidationContext(validatingObject)
                {
                    DisplayName = property.DisplayName,
                    MemberName = property.Name
                };

                foreach (ValidationAttribute attribute in validationAttributes)
                {
                    ValidationResult result =
                        attribute.GetValidationResult(property.GetValue(validatingObject), validationContext);
                    if (result != null)
                    {
                        validationErrors.Add(result);
                    }
                }
            }

            return validationErrors;
        }
    }
}
