namespace Kontecg.MultiCompany
{
    public class CompanyResolverCacheItem
    {
        public CompanyResolverCacheItem(int? companyId)
        {
            CompanyId = companyId;
        }

        public int? CompanyId { get; }
    }
}
