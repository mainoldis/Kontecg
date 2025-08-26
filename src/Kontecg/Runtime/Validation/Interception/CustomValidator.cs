using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Validation.Interception
{
    public class CustomValidator : IMethodParameterValidator
    {
        private readonly IIocResolver _iocResolver;

        public CustomValidator(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            if (validatingObject is ICustomValidate customValidateObject)
            {
                CustomValidationContext context = new CustomValidationContext(validationErrors, _iocResolver);
                customValidateObject.AddValidationErrors(context);
            }

            return validationErrors;
        }
    }
}
