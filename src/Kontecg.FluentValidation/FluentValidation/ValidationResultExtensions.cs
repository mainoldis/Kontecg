using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kontecg.FluentValidation
{
    public static class ValidationResultExtensions
    {
        public static ValidationResult ToDataAnnotationsValidationResult(
            this global::FluentValidation.Results.ValidationResult originalResult)
        {
            if (originalResult != null && originalResult.IsValid)
            {
                return new ValidationResult(null);
            }

            ValidationResult dataAnnotationsResult = new ValidationResult(originalResult.ToString());

            if (originalResult.Errors is {Count: > 0})
            {
                dataAnnotationsResult =
                    new ValidationResult(originalResult.ToString(),
                        originalResult.Errors.Select(err => err.PropertyName));
            }

            // Return created result
            return dataAnnotationsResult;
        }
    }
}
