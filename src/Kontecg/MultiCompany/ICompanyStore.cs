using JetBrains.Annotations;

namespace Kontecg.MultiCompany
{
    public interface ICompanyStore
    {
        [CanBeNull]
        CompanyInfo Find(int companyId);

        [CanBeNull]
        CompanyInfo Find([NotNull] string companyName);
    }
}
