using Kontecg.Workflows.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg Workflow module.
    /// </summary>
    public static class KontecgWorkflowConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg Workflow module.
        /// </summary>
        public static IKontecgWorkflowConfiguration UseWorkflows(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgWorkflowConfiguration>();
        }
    }
}
