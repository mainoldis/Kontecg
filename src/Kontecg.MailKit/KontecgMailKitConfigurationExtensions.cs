using Kontecg.Configuration.Startup;

namespace Kontecg.MailKit
{
    public static class KontecgMailKitConfigurationExtensions
    {
        public static IKontecgMailKitConfiguration UseMailKit(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgMailKitConfiguration>();
        }
    }
}
