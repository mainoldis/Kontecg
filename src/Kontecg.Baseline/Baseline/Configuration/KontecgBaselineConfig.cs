namespace Kontecg.Baseline.Configuration
{
    internal class KontecgBaselineConfig : IKontecgBaselineConfig
    {
        public KontecgBaselineConfig(
            IRoleManagementConfig roleManagementConfig,
            IUserManagementConfig userManagementConfig,
            ILanguageManagementConfig languageManagement,
            IKontecgBaselineEntityTypes entityTypes)
        {
            EntityTypes = entityTypes;
            RoleManagement = roleManagementConfig;
            UserManagement = userManagementConfig;
            LanguageManagement = languageManagement;
        }

        public IRoleManagementConfig RoleManagement { get; }

        public IUserManagementConfig UserManagement { get; }

        public ILanguageManagementConfig LanguageManagement { get; }

        public IKontecgBaselineEntityTypes EntityTypes { get; }
    }
}
