using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Configuration
{
    public class VisibleSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisibleAsync(IScopedIocResolver scope)
        {
            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }
}
