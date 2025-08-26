using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;

namespace Kontecg.Configuration
{
    public class RequiresAuthenticationSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisibleAsync(IScopedIocResolver scope)
        {
            return await Task.FromResult(
                scope.Resolve<IKontecgSession>().UserId.HasValue
            );
        }
    }
}
