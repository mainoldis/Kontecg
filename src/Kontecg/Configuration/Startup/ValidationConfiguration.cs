using System;
using System.Collections.Generic;
using Kontecg.Collections;
using Kontecg.Runtime.Validation.Interception;

namespace Kontecg.Configuration.Startup
{
    public class ValidationConfiguration : IValidationConfiguration
    {
        public ValidationConfiguration()
        {
            IgnoredTypes = new List<Type>();
            Validators = new TypeList<IMethodParameterValidator>();
        }

        public List<Type> IgnoredTypes { get; }

        public ITypeList<IMethodParameterValidator> Validators { get; }
    }
}
