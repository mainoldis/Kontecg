using Kontecg.Configuration.Startup;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    /// <summary>
    ///     Extension methods for module chenet configurations.
    /// </summary>
    public static class ModuleServicioAdminConfigurationExtensions
    {
        /// <summary>
        ///     Configures Kontecg Chenet ServicioAdmin module.
        /// </summary>
        /// <returns></returns>
        public static IKontecgServicioAdminModuleConfig UseServicioAdmin(
            this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.KontecgConfiguration.Get<IKontecgServicioAdminModuleConfig>();
        }
    }
}
