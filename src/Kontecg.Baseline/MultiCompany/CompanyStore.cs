namespace Kontecg.MultiCompany
{
    public class CompanyStore : ICompanyStore
    {
        private readonly ICompanyCache _companyCache;

        public CompanyStore(ICompanyCache companyCache)
        {
            _companyCache = companyCache;
        }

        public CompanyInfo Find(int companyId)
        {
            CompanyCacheItem company = _companyCache.GetOrNull(companyId);
            if (company == null)
            {
                return null;
            }

            return new CompanyInfo(company.Id, company.CompanyName);
        }

        public CompanyInfo Find(string companyName)
        {
            CompanyCacheItem company = _companyCache.GetOrNull(companyName);
            if (company == null)
            {
                return null;
            }

            return new CompanyInfo(company.Id, company.CompanyName);
        }
    }
}
