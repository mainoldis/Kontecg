using System;

namespace Kontecg.Runtime.Validation
{
    [Serializable]
    [Validator("BOOLEAN")]
    public class BooleanValueValidator : ValueValidatorBase
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                return true;
            }

            return bool.TryParse(value.ToString(), out _);
        }
    }
}
