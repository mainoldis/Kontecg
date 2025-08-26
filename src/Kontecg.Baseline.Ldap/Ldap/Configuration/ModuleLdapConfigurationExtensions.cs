using Kontecg.Configuration.Startup;

namespace Kontecg.Baseline.Ldap.Configuration
{
    /// <summary>
    ///     Extension methods for module baseline configurations.
    /// </summary>
    public static class ModuleLdapConfigurationExtensions
    {
        /// <summary>
        ///     Configures Kontecg LDAP module.
        /// </summary>
        /// <returns></returns>
        public static IKontecgLdapModuleConfig UseLdap(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.KontecgConfiguration.Get<IKontecgLdapModuleConfig>();
        }
    }
}
