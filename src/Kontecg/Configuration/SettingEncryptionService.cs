using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Runtime.Security;

namespace Kontecg.Configuration
{
    public class SettingEncryptionService : ISettingEncryptionService, ITransientDependency
    {
        private readonly ISettingsConfiguration _settingsConfiguration;

        public SettingEncryptionService(ISettingsConfiguration settingsConfiguration)
        {
            _settingsConfiguration = settingsConfiguration;
        }

        public string Encrypt(SettingDefinition settingDefinition, string plainValue)
        {
            return SimpleStringCipher.Instance.Encrypt(
                plainValue,
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultPassPhrase,
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultSalt,
                _settingsConfiguration.SettingEncryptionConfiguration.Keysize,
                _settingsConfiguration.SettingEncryptionConfiguration.InitVectorBytes,
                _settingsConfiguration.SettingEncryptionConfiguration.Iterations,
                _settingsConfiguration.SettingEncryptionConfiguration.HashAlgorithmName
            );
        }

        public string Decrypt(SettingDefinition settingDefinition, string encryptedValue)
        {
            return SimpleStringCipher.Instance.Decrypt(encryptedValue,
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultPassPhrase,
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultSalt,
                _settingsConfiguration.SettingEncryptionConfiguration.Keysize,
                _settingsConfiguration.SettingEncryptionConfiguration.InitVectorBytes,
                _settingsConfiguration.SettingEncryptionConfiguration.Iterations,
                _settingsConfiguration.SettingEncryptionConfiguration.HashAlgorithmName
            );
        }
    }
}
