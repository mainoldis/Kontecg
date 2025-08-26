using Kontecg.Configuration.Startup;

namespace Kontecg.EFCore.Configuration
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg EntityFramework
    ///     Core module.
    /// </summary>
    public static class KontecgEfCoreConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg EntityFramework Core module.
        /// </summary>
        public static IKontecgEfCoreConfiguration UseEfCore(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgEfCoreConfiguration>();
        }
    }
}
