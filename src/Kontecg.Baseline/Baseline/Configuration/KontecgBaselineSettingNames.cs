namespace Kontecg.Baseline.Configuration
{
    public static class KontecgBaselineSettingNames
    {
        public static class UserManagement
        {
            /// <summary>
            ///     "Kontecg.UserManagement.IsEmailConfirmationRequiredForLogin".
            /// </summary>
            public const string IsEmailConfirmationRequiredForLogin =
                "Kontecg.UserManagement.IsEmailConfirmationRequiredForLogin";

            public static class UserLockOut
            {
                /// <summary>
                ///     "Kontecg.UserManagement.UserLockOut.IsEnabled".
                /// </summary>
                public const string IsEnabled = "Kontecg.UserManagement.UserLockOut.IsEnabled";

                /// <summary>
                ///     "Kontecg.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout".
                /// </summary>
                public const string MaxFailedAccessAttemptsBeforeLockout =
                    "Kontecg.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout";

                /// <summary>
                ///     "Kontecg.UserManagement.UserLockOut.DefaultAccountLockoutSeconds".
                /// </summary>
                public const string DefaultAccountLockoutSeconds =
                    "Kontecg.UserManagement.UserLockOut.DefaultAccountLockoutSeconds";
            }

            public static class PasswordComplexity
            {
                /// <summary>
                ///     "Kontecg.UserManagement.PasswordComplexity.RequiredLength"
                /// </summary>
                public const string RequiredLength = "Kontecg.UserManagement.PasswordComplexity.RequiredLength";

                /// <summary>
                ///     "Kontecg.UserManagement.PasswordComplexity.RequireNonAlphanumeric"
                /// </summary>
                public const string RequireNonAlphanumeric =
                    "Kontecg.UserManagement.PasswordComplexity.RequireNonAlphanumeric";

                /// <summary>
                ///     "Kontecg.UserManagement.PasswordComplexity.RequireLowercase"
                /// </summary>
                public const string RequireLowercase =
                    "Kontecg.UserManagement.PasswordComplexity.RequireLowercase";

                /// <summary>
                ///     "Kontecg.UserManagement.PasswordComplexity.RequireUppercase"
                /// </summary>
                public const string RequireUppercase =
                    "Kontecg.UserManagement.PasswordComplexity.RequireUppercase";

                /// <summary>
                ///     "Kontecg.UserManagement.PasswordComplexity.RequireDigit"
                /// </summary>
                public const string RequireDigit = "Kontecg.UserManagement.PasswordComplexity.RequireDigit";
            }
        }
    }
}
