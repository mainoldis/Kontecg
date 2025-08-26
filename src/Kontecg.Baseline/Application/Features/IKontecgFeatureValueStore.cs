using System.Threading.Tasks;

namespace Kontecg.Application.Features
{
    public interface IKontecgFeatureValueStore : IFeatureValueStore
    {
        Task<string> GetValueOrNullAsync(int companyId, string featureName);

        string GetValueOrNull(int companyId, string featureName);

        Task<string> GetClientValueOrNullAsync(string clientId, string featureName);

        string GetClientValueOrNull(string clientId, string featureName);

        Task SetClientFeatureValueAsync(string clientId, string featureName, string value);

        void SetClientFeatureValue(string clientId, string featureName, string value);
    }
}
