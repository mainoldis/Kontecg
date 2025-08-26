using System;

namespace Kontecg.Events.Bus.Entities
{
    [Serializable]
    public class EntityChangeEntry
    {
        public EntityChangeEntry(object entity, EntityChangeType changeType)
        {
            Entity = entity;
            ChangeType = changeType;
        }

        public object Entity { get; set; }

        public EntityChangeType ChangeType { get; set; }
    }
}
