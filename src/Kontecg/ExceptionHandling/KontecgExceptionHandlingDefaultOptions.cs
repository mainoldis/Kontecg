using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontecg.ExceptionHandling
{
    public class KontecgExceptionHandlingDefaultOptions : IKontecgExceptionHandlingDefaultOptions
    {
        public static readonly List<Func<Type, bool>> ConventionalExceptionHandlingSelectorList = new();

        public KontecgExceptionHandlingDefaultOptions()
        {
            ConventionalExceptionHandlingSelectors = ConventionalExceptionHandlingSelectorList.ToList();
        }

        public List<Func<Type, bool>> ConventionalExceptionHandlingSelectors { get; protected set; }
    }
}
