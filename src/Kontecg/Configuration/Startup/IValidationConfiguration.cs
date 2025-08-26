using System;
using System.Collections.Generic;
using Kontecg.Collections;
using Kontecg.Runtime.Validation.Interception;

namespace Kontecg.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        List<Type> IgnoredTypes { get; }

        /// <summary>
        ///     A list of method parameter validators.
        /// </summary>
        ITypeList<IMethodParameterValidator> Validators { get; }
    }
}
