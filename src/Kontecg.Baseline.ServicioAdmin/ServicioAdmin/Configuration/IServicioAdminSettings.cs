using System.Threading.Tasks;

namespace Kontecg.Baseline.ServicioAdmin.Configuration
{
    /// <summary>
    ///     Used to obtain current values of ServicioAdmin settings.
    ///     This abstraction allows to define a different source for settings than SettingManager (see default implementation:
    ///     <see cref="ServicioAdminSettings" />).
    /// </summary>
    public interface IServicioAdminSettings
    {
        Task<bool> GetIsEnabledAsync(int? companyId);
    }
}
