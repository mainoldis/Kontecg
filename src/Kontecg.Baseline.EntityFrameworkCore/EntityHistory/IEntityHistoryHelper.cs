using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Kontecg.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);

        Task SaveAsync(EntityChangeSet changeSet);

        void Save(EntityChangeSet changeSet);
    }
}
