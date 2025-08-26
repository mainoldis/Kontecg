using Kontecg.Collections;

namespace Kontecg.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public SettingsConfiguration()
        {
            Providers = new TypeList<SettingProvider>();
            SettingEncryptionConfiguration = new SettingEncryptionConfiguration();
        }

        public SettingEncryptionConfiguration SettingEncryptionConfiguration { get; }

        public ITypeList<SettingProvider> Providers { get; }
    }
}
