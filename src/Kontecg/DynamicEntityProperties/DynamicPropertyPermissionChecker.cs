using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;

namespace Kontecg.DynamicEntityProperties
{
    public class DynamicPropertyPermissionChecker : IDynamicPropertyPermissionChecker, ITransientDependency
    {
        private readonly IDynamicPropertyManager _dynamicPropertyManager;
        private readonly IPermissionChecker _permissionChecker;

        public DynamicPropertyPermissionChecker(
            IPermissionChecker permissionChecker,
            IDynamicPropertyManager dynamicPropertyManager
        )
        {
            _permissionChecker = permissionChecker;
            _dynamicPropertyManager = dynamicPropertyManager;
        }

        public void CheckPermission(int dynamicPropertyId)
        {
            DynamicProperty dynamicProperty = _dynamicPropertyManager.Get(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!_permissionChecker.IsGranted(dynamicProperty.Permission))
            {
                throw new KontecgAuthorizationException($"Permission \"{dynamicProperty.Permission}\" is not granted");
            }
        }

        public async Task CheckPermissionAsync(int dynamicPropertyId)
        {
            DynamicProperty dynamicProperty = await _dynamicPropertyManager.GetAsync(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!await _permissionChecker.IsGrantedAsync(dynamicProperty.Permission))
            {
                throw new KontecgAuthorizationException($"Permission \"{dynamicProperty.Permission}\" is not granted");
            }
        }

        public bool IsGranted(int dynamicPropertyId)
        {
            DynamicProperty dynamicProperty = _dynamicPropertyManager.Get(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return _permissionChecker.IsGranted(dynamicProperty.Permission);
        }

        public async Task<bool> IsGrantedAsync(int dynamicPropertyId)
        {
            DynamicProperty dynamicProperty = await _dynamicPropertyManager.GetAsync(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return await _permissionChecker.IsGrantedAsync(dynamicProperty.Permission);
        }
    }
}
