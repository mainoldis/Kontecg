using Kontecg.Configuration.Startup;

namespace Kontecg.Baseline.Configuration
{
    /// <summary>
    ///     Extension methods for module Baseline configurations.
    /// </summary>
    public static class ModuleBaselineConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure module Baseline.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static IKontecgBaselineConfig UseBaseline(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.KontecgConfiguration.Get<IKontecgBaselineConfig>();
        }
    }
}
