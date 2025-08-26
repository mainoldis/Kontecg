using Kontecg.MemoryDb.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg MemoryDb module.
    /// </summary>
    public static class KontecgMemoryDbConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg MemoryDb module.
        /// </summary>
        public static IKontecgMemoryDbModuleConfiguration KontecgMemoryDb(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgMemoryDbModuleConfiguration>();
        }
    }
}
