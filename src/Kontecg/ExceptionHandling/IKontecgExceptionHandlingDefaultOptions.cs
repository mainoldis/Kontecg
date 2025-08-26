using System.Collections.Generic;
using System;

namespace Kontecg.ExceptionHandling
{
    public interface IKontecgExceptionHandlingDefaultOptions
    {
        /// <summary>
        ///     A list of selectors to determine conventional Exception Handling classes.
        /// </summary>
        List<Func<Type, bool>> ConventionalExceptionHandlingSelectors { get; }
    }
}
