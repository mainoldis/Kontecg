using Kontecg.Net.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg Networking module.
    /// </summary>
    public static class KontecgNetConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg Networking module.
        /// </summary>
        public static IKontecgNetModuleConfiguration UseNetworking(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgNetModuleConfiguration>();
        }
    }
}
