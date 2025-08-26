using System.Collections.Generic;
using System.Linq;
using Kontecg.MultiCompany;

namespace Kontecg.Tests.MultiCompany
{
    public class TestCompanyStore : ICompanyStore
    {
        private readonly List<CompanyInfo> _companies = new List<CompanyInfo>
        {
            new CompanyInfo(1, "Default")
        };

        public CompanyInfo Find(int companyId)
        {
            return _companies.FirstOrDefault(t => t.Id == companyId);
        }

        public CompanyInfo Find(string companyName)
        {
            return _companies.FirstOrDefault(t => t.CompanyName.ToLower() == companyName.ToLower());
        }
    }
}
