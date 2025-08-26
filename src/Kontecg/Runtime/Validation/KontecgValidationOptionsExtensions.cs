using System;
using System.Linq;

namespace Kontecg.Runtime.Validation
{
    internal static class KontecgValidationOptionsExtensions
    {
        public static bool IsConventionalValidationClass(this IKontecgValidationDefaultOptions options, Type type)
        {
            return options.ConventionalValidationSelectors.Any(selector => selector(type));
        }
    }
}
