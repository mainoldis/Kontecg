using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;

namespace Kontecg.Configuration
{
    public class RequiresPermissionSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        private readonly IPermissionDependency _permissionDependency;

        public RequiresPermissionSettingClientVisibilityProvider(IPermissionDependency permissionDependency)
        {
            _permissionDependency = permissionDependency;
        }

        public async Task<bool> CheckVisibleAsync(IScopedIocResolver scope)
        {
            IKontecgSession kontecgSession = scope.Resolve<IKontecgSession>();

            if (!kontecgSession.UserId.HasValue)
            {
                return false;
            }

            PermissionDependencyContext permissionDependencyContext = scope.Resolve<PermissionDependencyContext>();
            permissionDependencyContext.User = kontecgSession.ToUserIdentifier();

            return await _permissionDependency.IsSatisfiedAsync(permissionDependencyContext);
        }
    }
}
