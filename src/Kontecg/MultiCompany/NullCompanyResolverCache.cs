namespace Kontecg.MultiCompany
{
    public class NullCompanyResolverCache : ICompanyResolverCache
    {
        public CompanyResolverCacheItem Value
        {
            get => null;
            set { }
        }
    }
}
