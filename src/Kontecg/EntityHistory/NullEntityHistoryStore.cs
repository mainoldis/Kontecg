using System.Threading.Tasks;

namespace Kontecg.EntityHistory
{
    public class NullEntityHistoryStore : IEntityHistoryStore
    {
        public static NullEntityHistoryStore Instance { get; } = new();

        public Task SaveAsync(EntityChangeSet entityChangeSet)
        {
            return Task.CompletedTask;
        }

        public void Save(EntityChangeSet entityChangeSet)
        {
        }
    }
}
