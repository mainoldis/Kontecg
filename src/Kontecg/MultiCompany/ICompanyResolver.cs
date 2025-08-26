using System.Threading.Tasks;

namespace Kontecg.MultiCompany
{
    public interface ICompanyResolver
    {
        int? ResolveCompanyId();

        Task<int?> ResolveCompanyIdAsync();
    }
}
