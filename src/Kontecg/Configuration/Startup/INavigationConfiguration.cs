using Kontecg.Application.Navigation;
using Kontecg.Collections;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Used to configure navigation.
    /// </summary>
    public interface INavigationConfiguration
    {
        /// <summary>
        ///     List of navigation providers.
        /// </summary>
        ITypeList<NavigationProvider> Providers { get; }
    }
}
