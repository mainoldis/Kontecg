using Kontecg.Runtime.Caching;

namespace Kontecg.Domain.Entities.Caching
{
    public interface IEntityCache<TCacheItem> : IEntityCache<TCacheItem, int>
    {
    }

    public interface IEntityCache<TCacheItem, TPrimaryKey> : IEntityCacheBase<TCacheItem, TPrimaryKey>
    {
        ITypedCache<TPrimaryKey, TCacheItem> InternalCache { get; }
    }
}
