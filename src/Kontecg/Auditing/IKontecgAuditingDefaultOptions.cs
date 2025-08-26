using System;
using System.Collections.Generic;

namespace Kontecg.Auditing
{
    public interface IKontecgAuditingDefaultOptions
    {
        /// <summary>
        ///     A list of selectors to determine conventional Auditing classes.
        /// </summary>
        List<Func<Type, bool>> ConventionalAuditingSelectors { get; }
    }
}
