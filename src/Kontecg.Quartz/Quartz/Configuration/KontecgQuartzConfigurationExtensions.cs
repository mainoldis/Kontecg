using Kontecg.Configuration.Startup;

namespace Kontecg.Quartz.Configuration
{
    public static class KontecgQuartzConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg Quartz module.
        /// </summary>
        public static IKontecgQuartzConfiguration KontecgQuartz(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgQuartzConfiguration>();
        }
    }
}
