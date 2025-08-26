using Kontecg.Collections;

namespace Kontecg.Baseline.Configuration
{
    /// <summary>
    ///     User management configuration.
    /// </summary>
    public interface IUserManagementConfig
    {
        ITypeList<object> ExternalAuthenticationSources { get; set; }
    }
}
