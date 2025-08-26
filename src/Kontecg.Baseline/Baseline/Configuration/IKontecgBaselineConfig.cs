namespace Kontecg.Baseline.Configuration
{
    /// <summary>
    ///     Configuration options for Baseline module.
    /// </summary>
    public interface IKontecgBaselineConfig
    {
        /// <summary>
        ///     Gets role management config.
        /// </summary>
        IRoleManagementConfig RoleManagement { get; }

        /// <summary>
        ///     Gets user management config.
        /// </summary>
        IUserManagementConfig UserManagement { get; }

        /// <summary>
        ///     Gets language management config.
        /// </summary>
        ILanguageManagementConfig LanguageManagement { get; }

        /// <summary>
        ///     Gets entity type config.
        /// </summary>
        IKontecgBaselineEntityTypes EntityTypes { get; }
    }
}
