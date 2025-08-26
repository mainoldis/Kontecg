using Kontecg.MachineLearning.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg.ML module.
    /// </summary>
    public static class KontecgMachineLearningConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg.ML module.
        /// </summary>
        public static IKontecgMachineLearningConfiguration UseMachineLearning(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgMachineLearningConfiguration>();
        }
    }
}
