using JetBrains.Annotations;

namespace Kontecg.MultiCompany
{
    public interface ICompanyResolverCache
    {
        [CanBeNull] CompanyResolverCacheItem Value { get; set; }
    }
}
