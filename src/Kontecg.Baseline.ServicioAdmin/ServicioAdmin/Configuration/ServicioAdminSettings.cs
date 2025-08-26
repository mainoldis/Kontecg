using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    /// <summary>
    ///     Implements <see cref="IServicioAdminSettings" /> to get settings from <see cref="ISettingManager" />.
    /// </summary>
    public class ServicioAdminSettings : IServicioAdminSettings, ITransientDependency
    {
        public ServicioAdminSettings(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        protected ISettingManager SettingManager { get; }

        public virtual Task<bool> GetIsEnabledAsync(int? companyId)
        {
            return companyId.HasValue
                ? SettingManager.GetSettingValueForCompanyAsync<bool>(ServicioAdminSettingNames.IsEnabled,
                    companyId.Value)
                : Task.FromResult(false);
        }
    }
}
