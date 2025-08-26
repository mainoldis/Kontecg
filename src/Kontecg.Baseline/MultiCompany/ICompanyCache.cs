using System.Threading.Tasks;

namespace Kontecg.MultiCompany
{
    public interface ICompanyCache
    {
        CompanyCacheItem Get(int companyId);

        CompanyCacheItem Get(string companyName);

        CompanyCacheItem GetOrNull(string companyName);

        CompanyCacheItem GetOrNull(int companyId);

        Task<CompanyCacheItem> GetAsync(int companyId);

        Task<CompanyCacheItem> GetAsync(string companyName);

        Task<CompanyCacheItem> GetOrNullAsync(string companyName);

        Task<CompanyCacheItem> GetOrNullAsync(int companyId);
    }
}
