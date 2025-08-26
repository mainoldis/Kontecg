using System.Collections.Generic;
using Kontecg.Configuration;
using Kontecg.Localization;

namespace Kontecg.Baseline.Configuration
{
    public class KontecgBaselineSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new List<SettingDefinition>
            {
                new(
                    KontecgBaselineSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                    "false",
                    new FixedLocalizableString("Is email confirmation required for login."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.UserLockOut.IsEnabled,
                    "true",
                    new FixedLocalizableString("Is user lockout enabled."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                    "5",
                    new FixedLocalizableString("Maximum Failed access attempt count before user lockout."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    "300", //5 minutes
                    new FixedLocalizableString("User lockout in seconds."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                    "false",
                    new FixedLocalizableString("Require digit."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                    "false",
                    new FixedLocalizableString("Require lowercase."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                    "false",
                    new FixedLocalizableString("Require non alphanumeric."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                    "false",
                    new FixedLocalizableString("Require upper case."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),

                new(
                    KontecgBaselineSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                    "8",
                    new FixedLocalizableString("Required length."),
                    scopes: SettingScopes.Application | SettingScopes.Company,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                )
            };
        }
    }
}
