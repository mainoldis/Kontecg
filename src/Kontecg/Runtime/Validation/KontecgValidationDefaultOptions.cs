using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Application.Services;

namespace Kontecg.Runtime.Validation
{
    public class KontecgValidationDefaultOptions : IKontecgValidationDefaultOptions
    {
        public static List<Func<Type, bool>> ConventionalValidationSelectorList = new()
        {
            type => typeof(IApplicationService).IsAssignableFrom(type)
        };

        public KontecgValidationDefaultOptions()
        {
            ConventionalValidationSelectors = ConventionalValidationSelectorList.ToList();
        }

        public List<Func<Type, bool>> ConventionalValidationSelectors { get; protected set; }
    }
}
