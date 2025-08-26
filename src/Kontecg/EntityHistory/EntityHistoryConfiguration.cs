using System;
using System.Collections.Generic;

namespace Kontecg.EntityHistory
{
    internal class EntityHistoryConfiguration : IEntityHistoryConfiguration
    {
        public EntityHistoryConfiguration()
        {
            IsEnabled = true;
            Selectors = new EntityHistorySelectorList();
            IgnoredTypes = new List<Type>
            {
                typeof(EntityChangeSet),
                typeof(EntityChange),
                typeof(EntityPropertyChange)
            };
        }

        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IEntityHistorySelectorList Selectors { get; }

        public List<Type> IgnoredTypes { get; }
    }
}
