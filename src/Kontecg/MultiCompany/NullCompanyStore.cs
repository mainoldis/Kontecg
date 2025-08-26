namespace Kontecg.MultiCompany
{
    public class NullCompanyStore : ICompanyStore
    {
        public CompanyInfo Find(int companyId)
        {
            return null;
        }

        public CompanyInfo Find(string companyName)
        {
            return null;
        }
    }
}
