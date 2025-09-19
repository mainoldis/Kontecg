using Kontecg.MassTransit.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg MassTransit module.
    /// </summary>
    public static class KontecgMassTransitConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg MassTransit module.
        /// </summary>
        public static IKontecgMassTransitModuleConfiguration UseMassTransit(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgMassTransitModuleConfiguration>();
        }
    }
}
