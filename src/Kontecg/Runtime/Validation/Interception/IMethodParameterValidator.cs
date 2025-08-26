using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Validation.Interception
{
    /// <summary>
    ///     This interface is used to validate method parameters.
    /// </summary>
    public interface IMethodParameterValidator : ITransientDependency
    {
        IReadOnlyList<ValidationResult> Validate(object validatingObject);
    }
}
