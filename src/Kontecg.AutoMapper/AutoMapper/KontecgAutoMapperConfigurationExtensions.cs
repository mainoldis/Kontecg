using Kontecg.Configuration.Startup;

namespace Kontecg.AutoMapper
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg.AutoMapper module.
    /// </summary>
    public static class KontecgAutoMapperConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg.AutoMapper module.
        /// </summary>
        public static IKontecgAutoMapperConfiguration UseAutoMapper(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgAutoMapperConfiguration>();
        }
    }
}
