using Kontecg.Configuration.Startup;

namespace Kontecg.FluentValidation.Configuration
{
    public static class KontecgFluentValidationConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg.FluentValidation module.
        /// </summary>
        public static IKontecgFluentValidationConfiguration UseFluentValidation(
            this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgFluentValidationConfiguration>();
        }
    }
}
