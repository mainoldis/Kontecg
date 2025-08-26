using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the main interface for managing configuration settings in the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ISettingManager is the core interface that provides comprehensive functionality for loading,
    /// changing, and managing configuration settings across different scopes in the application.
    /// It supports a hierarchical setting system with application, company, and user-level settings.
    /// </para>
    /// <para>
    /// <strong>Setting Hierarchy:</strong> The framework supports a multi-level setting system:
    /// <list type="bullet">
    /// <item><description><strong>Application Level:</strong> Global settings for the entire application</description></item>
    /// <item><description><strong>Company Level:</strong> Settings specific to individual companies (multi-tenant)</description></item>
    /// <item><description><strong>User Level:</strong> Personal settings for individual users</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Setting Resolution:</strong> Settings are resolved using a hierarchical approach:
    /// <list type="number">
    /// <item><description>User-specific settings (highest priority)</description></item>
    /// <item><description>Company-specific settings</description></item>
    /// <item><description>Application-level settings</description></item>
    /// <item><description>Default values (lowest priority)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Multi-Scope Support:</strong> Application, company, and user-level settings</description></item>
    /// <item><description><strong>Async Operations:</strong> Full support for asynchronous operations</description></item>
    /// <item><description><strong>Fallback Logic:</strong> Automatic fallback to default values</description></item>
    /// <item><description><strong>Bulk Operations:</strong> Get all settings for a scope</description></item>
    /// <item><description><strong>Type Safety:</strong> Strongly typed setting values</description></item>
    /// <item><description><strong>Caching:</strong> Built-in caching for performance</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Configuration Management:</strong> Centralized application configuration</description></item>
    /// <item><description><strong>Multi-Tenancy:</strong> Company-specific settings in multi-tenant applications</description></item>
    /// <item><description><strong>User Preferences:</strong> Personal user settings and preferences</description></item>
    /// <item><description><strong>Feature Flags:</strong> Enable/disable features per scope</description></item>
    /// <item><description><strong>System Configuration:</strong> Database connections, API endpoints, etc.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Settings are cached at multiple levels for optimal performance</description></item>
    /// <item><description>Async methods are available for non-blocking operations</description></item>
    /// <item><description>Bulk operations reduce database round trips</description></item>
    /// <item><description>Cache invalidation ensures data consistency</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> All methods are thread-safe and can be called from multiple
    /// threads concurrently. The implementation uses appropriate synchronization mechanisms
    /// to ensure data consistency.
    /// </para>
    /// </remarks>
    public interface ISettingManager
    {
        /// <summary>
        /// Gets the current value of a setting using the current session context.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <returns>
        /// The current value of the setting, resolved through the hierarchy (user → company → application → default).
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value using the current session context, which includes
        /// the current user and company. The setting is resolved through the hierarchical system,
        /// where user settings override company settings, which override application settings.
        /// </para>
        /// <para>
        /// <strong>Resolution Order:</strong>
        /// <list type="number">
        /// <item><description>User-specific setting (if current user has set a value)</description></item>
        /// <item><description>Company-specific setting (if current company has set a value)</description></item>
        /// <item><description>Application-level setting (if set globally)</description></item>
        /// <item><description>Default value (from setting definition)</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Session Context:</strong> Uses the current session to determine the user and company
        /// context for setting resolution. If no session is available, falls back to application-level settings.
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueAsync(string name);

        /// <summary>
        /// Gets the current value of a setting using the current session context (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <returns>
        /// The current value of the setting, resolved through the hierarchy (user → company → application → default).
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueAsync(string)"/>. This method performs
        /// the same setting resolution but blocks the calling thread until the operation completes.
        /// </para>
        /// <para>
        /// <strong>Performance Note:</strong> Use the async version for better performance in web applications
        /// and other scenarios where blocking operations should be avoided.
        /// </para>
        /// </remarks>
        string GetSettingValue(string name);

        /// <summary>
        /// Gets the current value of a setting for the application level.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <returns>
        /// The application-level value of the setting, or the default value if not set at application level.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves only the application-level setting value, ignoring any company or user
        /// overrides. If no application-level value is set, it returns the default value from the setting definition.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>System-wide configuration that should not be overridden</description></item>
        /// <item><description>Default values for new companies or users</description></item>
        /// <item><description>Administrative configuration management</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForApplicationAsync(string name);

        /// <summary>
        /// Gets the current value of a setting for the application level (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <returns>
        /// The application-level value of the setting, or the default value if not set at application level.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForApplicationAsync(string)"/>. This method
        /// retrieves only the application-level setting value without considering company or user overrides.
        /// </para>
        /// </remarks>
        string GetSettingValueForApplication(string name);

        /// <summary>
        /// Gets the current value of a setting for the application level with optional fallback control.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns the default value when no application-level setting is found.
        /// If false, returns null when no application-level setting is found.
        /// </param>
        /// <returns>
        /// The application-level value of the setting, or the default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides control over whether to fall back to the default value when no
        /// application-level setting is explicitly set. This is useful for distinguishing between
        /// "not set" and "set to default value" scenarios.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>If fallbackToDefault is true: Returns application value or default value</description></item>
        /// <item><description>If fallbackToDefault is false: Returns application value or null</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForApplicationAsync(string name, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for the application level with optional fallback control (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns the default value when no application-level setting is found.
        /// If false, returns null when no application-level setting is found.
        /// </param>
        /// <returns>
        /// The application-level value of the setting, or the default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForApplicationAsync(string, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForApplication(string name, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for a specific company.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve the setting value.
        /// </param>
        /// <returns>
        /// The company-specific value of the setting, or the application/default value if not set for the company.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value for a specific company, falling back to application-level
        /// settings and then default values if no company-specific setting is found.
        /// </para>
        /// <para>
        /// <strong>Multi-Tenancy:</strong> This method is essential for multi-tenant applications where
        /// different companies may have different configuration requirements.
        /// </para>
        /// <para>
        /// <strong>Resolution Order:</strong>
        /// <list type="number">
        /// <item><description>Company-specific setting (if set for the specified company)</description></item>
        /// <item><description>Application-level setting (if set globally)</description></item>
        /// <item><description>Default value (from setting definition)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForCompanyAsync(string name, int companyId);

        /// <summary>
        /// Gets the current value of a setting for a specific company (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve the setting value.
        /// </param>
        /// <returns>
        /// The company-specific value of the setting, or the application/default value if not set for the company.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForCompanyAsync(string, int)"/>. This method
        /// retrieves company-specific settings but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForCompany(string name, int companyId);

        /// <summary>
        /// Gets the current value of a setting for a specific company with optional fallback control.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve the setting value.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns application/default value when no company-specific setting is found.
        /// If false, returns null when no company-specific setting is found.
        /// </param>
        /// <returns>
        /// The company-specific value of the setting, or application/default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides control over whether to fall back to application-level settings when no
        /// company-specific setting is explicitly set. This is useful for distinguishing between
        /// "not set for company" and "using application default" scenarios.
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForCompanyAsync(string name, int companyId, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for a specific company with optional fallback control (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve the setting value.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns application/default value when no company-specific setting is found.
        /// If false, returns null when no company-specific setting is found.
        /// </param>
        /// <returns>
        /// The company-specific value of the setting, or application/default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForCompanyAsync(string, int, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForCompany(string name, int companyId, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for a specific user.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company that the user belongs to. Can be null for system users.
        /// </param>
        /// <param name="userId">
        /// The ID of the user for which to retrieve the setting value.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value if not set for the user.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value for a specific user, falling back through the hierarchy:
        /// user → company → application → default.
        /// </para>
        /// <para>
        /// <strong>User Preferences:</strong> This method is essential for user-specific settings like
        /// UI preferences, notification settings, and personal configurations.
        /// </para>
        /// <para>
        /// <strong>Resolution Order:</strong>
        /// <list type="number">
        /// <item><description>User-specific setting (if set for the specified user)</description></item>
        /// <item><description>Company-specific setting (if user belongs to a company)</description></item>
        /// <item><description>Application-level setting (if set globally)</description></item>
        /// <item><description>Default value (from setting definition)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForUserAsync(string name, int? companyId, long userId);

        /// <summary>
        /// Gets the current value of a setting for a specific user (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company that the user belongs to. Can be null for system users.
        /// </param>
        /// <param name="userId">
        /// The ID of the user for which to retrieve the setting value.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value if not set for the user.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForUserAsync(string, int?, long)"/>. This method
        /// retrieves user-specific settings but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForUser(string name, int? companyId, long userId);

        /// <summary>
        /// Gets the current value of a setting for a specific user with optional fallback control.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company that the user belongs to. Can be null for system users.
        /// </param>
        /// <param name="userId">
        /// The ID of the user for which to retrieve the setting value.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns company/application/default value when no user-specific setting is found.
        /// If false, returns null when no user-specific setting is found.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides control over whether to fall back to company/application settings when no
        /// user-specific setting is explicitly set. This is useful for distinguishing between
        /// "not set for user" and "using company/application default" scenarios.
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForUserAsync(string name, int? companyId, long userId, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for a specific user with optional fallback control (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="companyId">
        /// The ID of the company that the user belongs to. Can be null for system users.
        /// </param>
        /// <param name="userId">
        /// The ID of the user for which to retrieve the setting value.
        /// </param>
        /// <param name="fallbackToDefault">
        /// If true, returns company/application/default value when no user-specific setting is found.
        /// If false, returns null when no user-specific setting is found.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value/null based on fallbackToDefault parameter.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForUserAsync(string, int?, long, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForUser(string name, int? companyId, long userId, bool fallbackToDefault);

        /// <summary>
        /// Gets the current value of a setting for a specific user using UserIdentifier.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value if not set for the user.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is a convenience overload that uses a UserIdentifier object instead of separate
        /// companyId and userId parameters. It provides the same functionality as the other user-specific
        /// methods but with a more structured approach.
        /// </para>
        /// <para>
        /// <strong>UserIdentifier:</strong> The UserIdentifier object encapsulates both the user ID and
        /// company ID, making it easier to pass user context around the application.
        /// </para>
        /// </remarks>
        Task<string> GetSettingValueForUserAsync(string name, UserIdentifier user);

        /// <summary>
        /// Gets the current value of a setting for a specific user using UserIdentifier (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve. Must match a defined setting name.
        /// </param>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <returns>
        /// The user-specific value of the setting, or company/application/default value if not set for the user.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForUserAsync(string, UserIdentifier)"/>. This method
        /// uses UserIdentifier for convenience but executes synchronously.
        /// </para>
        /// </remarks>
        string GetSettingValueForUser(string name, UserIdentifier user);

        /// <summary>
        /// Gets all setting values for the current session context.
        /// </summary>
        /// <returns>
        /// A read-only list of all setting values, resolved through the hierarchy for the current user and company.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves all defined settings with their current values, resolved through the
        /// hierarchical system for the current session context (user and company).
        /// </para>
        /// <para>
        /// <strong>Performance Note:</strong> This method can be expensive as it retrieves all settings.
        /// Consider using scope-specific methods if you only need settings for a particular level.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Configuration UI that shows all current settings</description></item>
        /// <item><description>Setting export/backup functionality</description></item>
        /// <item><description>System diagnostics and troubleshooting</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync();

        /// <summary>
        /// Gets all setting values for the current session context (synchronous version).
        /// </summary>
        /// <returns>
        /// A read-only list of all setting values, resolved through the hierarchy for the current user and company.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesAsync()"/>. This method retrieves all
        /// settings but executes synchronously.
        /// </para>
        /// </remarks>
        IReadOnlyList<ISettingValue> GetAllSettingValues();

        /// <summary>
        /// Gets all setting values for specified scopes.
        /// </summary>
        /// <param name="scopes">
        /// The scopes for which to retrieve settings. Can be a combination of Application, Company, and User.
        /// </param>
        /// <returns>
        /// A read-only list of all setting values, resolved through the specified scopes.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method allows you to specify which scopes to include when retrieving setting values.
        /// This is useful when you need settings for specific levels without the full hierarchy.
        /// </para>
        /// <para>
        /// <strong>Scope Combinations:</strong> You can combine scopes using bitwise OR operations:
        /// <list type="bullet">
        /// <item><description>SettingScopes.Application - Only application-level settings</description></item>
        /// <item><description>SettingScopes.Company - Only company-level settings</description></item>
        /// <item><description>SettingScopes.User - Only user-level settings</description></item>
        /// <item><description>SettingScopes.Application | SettingScopes.Company - Application and company settings</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync(SettingScopes scopes);

        /// <summary>
        /// Gets all setting values for specified scopes (synchronous version).
        /// </summary>
        /// <param name="scopes">
        /// The scopes for which to retrieve settings. Can be a combination of Application, Company, and User.
        /// </param>
        /// <returns>
        /// A read-only list of all setting values, resolved through the specified scopes.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesAsync(SettingScopes)"/>. This method
        /// allows scope specification but executes synchronously.
        /// </para>
        /// </remarks>
        IReadOnlyList<ISettingValue> GetAllSettingValues(SettingScopes scopes);

        /// <summary>
        /// Gets all setting values explicitly set for the application level.
        /// </summary>
        /// <returns>
        /// A read-only list of setting values that are explicitly set at the application level.
        /// Settings using default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns only settings that have been explicitly set at the application level.
        /// Settings that are using their default values are not included in the result.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Administrative configuration management</description></item>
        /// <item><description>Configuration auditing and review</description></item>
        /// <item><description>Setting migration and cleanup</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForApplicationAsync();

        /// <summary>
        /// Gets all setting values explicitly set for the application level (synchronous version).
        /// </summary>
        /// <returns>
        /// A read-only list of setting values that are explicitly set at the application level.
        /// Settings using default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesForApplicationAsync()"/>. This method
        /// returns only explicitly set application-level settings.
        /// </para>
        /// </remarks>
        IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication();

        /// <summary>
        /// Gets all setting values explicitly set for a specific company.
        /// </summary>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve explicitly set settings.
        /// </param>
        /// <returns>
        /// A read-only list of setting values that are explicitly set for the specified company.
        /// Settings using application/default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns only settings that have been explicitly set for the specified company.
        /// Settings that are using application-level or default values are not included in the result.
        /// </para>
        /// <para>
        /// <strong>Multi-Tenancy:</strong> This method is useful for managing company-specific
        /// configurations in multi-tenant applications.
        /// </para>
        /// </remarks>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForCompanyAsync(int companyId);

        /// <summary>
        /// Gets all setting values explicitly set for a specific company (synchronous version).
        /// </summary>
        /// <param name="companyId">
        /// The ID of the company for which to retrieve explicitly set settings.
        /// </param>
        /// <returns>
        /// A read-only list of setting values that are explicitly set for the specified company.
        /// Settings using application/default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesForCompanyAsync(int)"/>. This method
        /// returns only explicitly set company-level settings.
        /// </para>
        /// </remarks>
        IReadOnlyList<ISettingValue> GetAllSettingValuesForCompany(int companyId);

        /// <summary>
        /// Gets all setting values explicitly set for a specific user.
        /// </summary>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <returns>
        /// A read-only list of setting values that are explicitly set for the specified user.
        /// Settings using company/application/default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns only settings that have been explicitly set for the specified user.
        /// Settings that are using company-level, application-level, or default values are not included.
        /// </para>
        /// <para>
        /// <strong>User Preferences:</strong> This method is useful for managing user-specific
        /// preferences and configurations.
        /// </para>
        /// </remarks>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(UserIdentifier user);

        /// <summary>
        /// Gets all setting values explicitly set for a specific user (synchronous version).
        /// </summary>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <returns>
        /// A read-only list of setting values that are explicitly set for the specified user.
        /// Settings using company/application/default values are not included.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesForUserAsync(UserIdentifier)"/>. This method
        /// returns only explicitly set user-level settings.
        /// </para>
        /// </remarks>
        IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(UserIdentifier user);

        /// <summary>
        /// Changes a setting value for the application level.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the application-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method sets a setting value at the application level, which will be used as the
        /// default for all companies and users unless overridden at a lower level.
        /// </para>
        /// <para>
        /// <strong>Impact:</strong> Changes to application-level settings affect all companies and users
        /// that don't have their own specific values set.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The setting value will be validated according to the setting definition
        /// before being saved.
        /// </para>
        /// </remarks>
        Task ChangeSettingForApplicationAsync(string name, string value);

        /// <summary>
        /// Changes a setting value for the application level (synchronous version).
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the application-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="ChangeSettingForApplicationAsync(string, string)"/>. This method
        /// sets application-level settings but executes synchronously.
        /// </para>
        /// </remarks>
        void ChangeSettingForApplication(string name, string value);

        /// <summary>
        /// Changes a setting value for a specific company.
        /// </summary>
        /// <param name="companyId">
        /// The ID of the company for which to change the setting value.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the company-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method sets a setting value for a specific company, which will override the
        /// application-level setting for all users in that company.
        /// </para>
        /// <para>
        /// <strong>Multi-Tenancy:</strong> This method is essential for providing company-specific
        /// configurations in multi-tenant applications.
        /// </para>
        /// <para>
        /// <strong>Scope Validation:</strong> The setting must be configured to support company-level scope
        /// in its definition.
        /// </para>
        /// </remarks>
        Task ChangeSettingForCompanyAsync(int companyId, string name, string value);

        /// <summary>
        /// Changes a setting value for a specific company (synchronous version).
        /// </summary>
        /// <param name="companyId">
        /// The ID of the company for which to change the setting value.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the company-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="ChangeSettingForCompanyAsync(int, string, string)"/>. This method
        /// sets company-level settings but executes synchronously.
        /// </para>
        /// </remarks>
        void ChangeSettingForCompany(int companyId, string name, string value);

        /// <summary>
        /// Changes a setting value for a specific user.
        /// </summary>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the user-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method sets a setting value for a specific user, which will override company and
        /// application-level settings for that user only.
        /// </para>
        /// <para>
        /// <strong>User Preferences:</strong> This method is essential for allowing users to customize
        /// their personal settings and preferences.
        /// </para>
        /// <para>
        /// <strong>Scope Validation:</strong> The setting must be configured to support user-level scope
        /// in its definition.
        /// </para>
        /// </remarks>
        Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value);

        /// <summary>
        /// Changes a setting value for a specific user (synchronous version).
        /// </summary>
        /// <param name="user">
        /// The UserIdentifier object containing user and company information.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to change. Must match a defined setting name.
        /// </param>
        /// <param name="value">
        /// The new value to set for the user-level setting.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="ChangeSettingForUserAsync(UserIdentifier, string, string)"/>. This method
        /// sets user-level settings but executes synchronously.
        /// </para>
        /// </remarks>
        void ChangeSettingForUser(UserIdentifier user, string name, string value);
    }
}
