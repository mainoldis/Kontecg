using Kontecg.Application.Navigation;
using Kontecg.Collections;

namespace Kontecg.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public NavigationConfiguration()
        {
            Providers = new TypeList<NavigationProvider>();
        }

        public ITypeList<NavigationProvider> Providers { get; }
    }
}
