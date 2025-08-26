using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Application.Services;

namespace Kontecg.Auditing
{
    public class KontecgAuditingDefaultOptions : IKontecgAuditingDefaultOptions
    {
        public static List<Func<Type, bool>> ConventionalAuditingSelectorList = new()
        {
            type => typeof(IApplicationService).IsAssignableFrom(type)
        };

        public KontecgAuditingDefaultOptions()
        {
            ConventionalAuditingSelectors = ConventionalAuditingSelectorList.ToList();
        }

        public List<Func<Type, bool>> ConventionalAuditingSelectors { get; protected set; }
    }
}
