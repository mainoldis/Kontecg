using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using Kontecg.Runtime.Validation.Interception;

namespace Kontecg.FluentValidation
{
    public class FluentValidationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        public FluentValidationMethodParameterValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            IValidator fvValidator = _validatorFactory.GetValidator(validatingObject.GetType());

            if (fvValidator != null)
            {
                ValidationContext<object> validationContext = new ValidationContext<object>(validatingObject);
                global::FluentValidation.Results.ValidationResult validationResult =
                    fvValidator.Validate(validationContext);

                List<ValidationResult> mappedValidationErrors = validationResult.Errors
                    .Select(e => new ValidationResult(e.ErrorMessage, new[] {e.PropertyName}))
                    .ToList();

                validationErrors.AddRange(mappedValidationErrors);
            }

            return validationErrors;
        }
    }
}
