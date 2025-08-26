using Kontecg.Runtime.Caching;

namespace Kontecg.Domain.Entities.Caching
{
    public interface IMultiCompanyEntityCache<TCacheItem> : IMultiCompanyEntityCache<TCacheItem, int>
    {
    }

    public interface IMultiCompanyEntityCache<TCacheItem, TPrimaryKey> : IEntityCacheBase<TCacheItem, TPrimaryKey>
    {
        ITypedCache<string, TCacheItem> InternalCache { get; }

        string GetCacheKey(TPrimaryKey id);

        string GetCacheKey(TPrimaryKey id, int? companyId);
    }
}
