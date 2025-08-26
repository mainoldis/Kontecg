using System;
using System.Collections.Generic;

namespace Kontecg.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public AuditingConfiguration()
        {
            IsEnabled = true;
            Selectors = new AuditingSelectorList();
            IgnoredTypes = new List<Type>();
            SaveReturnValues = false;
        }

        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IAuditingSelectorList Selectors { get; }

        public List<Type> IgnoredTypes { get; }

        public bool SaveReturnValues { get; set; }
    }
}
