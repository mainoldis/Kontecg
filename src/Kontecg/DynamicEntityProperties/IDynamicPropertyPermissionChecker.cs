using System.Threading.Tasks;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicPropertyPermissionChecker
    {
        void CheckPermission(int dynamicPropertyId);

        Task CheckPermissionAsync(int dynamicPropertyId);

        bool IsGranted(int dynamicPropertyId);

        Task<bool> IsGrantedAsync(int dynamicPropertyId);
    }
}
