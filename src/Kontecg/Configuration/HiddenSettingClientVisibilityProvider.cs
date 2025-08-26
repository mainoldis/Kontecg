using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Configuration
{
    public class HiddenSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisibleAsync(IScopedIocResolver scope)
        {
            return await Task.FromResult(false);
        }
    }
}
