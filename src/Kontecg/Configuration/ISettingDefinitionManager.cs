using System.Collections.Generic;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the contract for managing setting definitions and metadata in the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ISettingDefinitionManager is responsible for managing the metadata and definitions of all
    /// configuration settings in the application. It provides access to setting definitions that
    /// contain information about setting names, default values, scopes, validation rules, and other
    /// metadata required for proper setting management.
    /// </para>
    /// <para>
    /// <strong>Key Responsibilities:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Setting Metadata:</strong> Manage setting definitions with names, descriptions, and default values</description></item>
    /// <item><description><strong>Scope Management:</strong> Define which scopes (Application, Company, User) each setting supports</description></item>
    /// <item><description><strong>Validation Rules:</strong> Provide validation logic and constraints for setting values</description></item>
    /// <item><description><strong>Default Values:</strong> Supply default values for settings when not explicitly set</description></item>
    /// <item><description><strong>Setting Discovery:</strong> Enable discovery of all available settings in the system</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Setting Definition Components:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Name:</strong> Unique identifier for the setting</description></item>
    /// <item><description><strong>Default Value:</strong> Default value used when setting is not explicitly set</description></item>
    /// <item><description><strong>Scopes:</strong> Which levels (Application, Company, User) the setting can be set at</description></item>
    /// <item><description><strong>Display Name:</strong> Human-readable name for UI display</description></item>
    /// <item><description><strong>Description:</strong> Detailed description of the setting's purpose</description></item>
    /// <item><description><strong>Group:</strong> Logical grouping for organization</description></item>
    /// <item><description><strong>Validation:</strong> Validation rules and constraints</description></item>
    /// <item><description><strong>Client Visibility:</strong> Whether the setting is visible to client applications</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Setting Registration:</strong> Register new settings during application startup</description></item>
    /// <item><description><strong>Setting Validation:</strong> Validate setting values against their definitions</description></item>
    /// <item><description><strong>Configuration UI:</strong> Generate configuration interfaces based on setting definitions</description></item>
    /// <item><description><strong>Default Value Resolution:</strong> Provide default values when settings are not set</description></item>
    /// <item><description><strong>Setting Discovery:</strong> Discover all available settings for documentation or management</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Setting definitions should be registered early in the application lifecycle</description></item>
    /// <item><description>Consider caching setting definitions for performance</description></item>
    /// <item><description>Implement proper error handling for missing or invalid setting definitions</description></item>
    /// <item><description>Support dynamic setting registration for extensibility</description></item>
    /// <item><description>Provide validation for setting definition consistency</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> Implementations should be thread-safe and support
    /// concurrent access from multiple threads. Setting definitions are typically read-only
    /// after initialization, which simplifies thread safety requirements.
    /// </para>
    /// </remarks>
    public interface ISettingDefinitionManager
    {
        /// <summary>
        /// Gets the <see cref="SettingDefinition"/> object with the given unique name.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting to retrieve the definition for.
        /// </param>
        /// <returns>
        /// The <see cref="SettingDefinition"/> object containing the setting's metadata and configuration.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the setting definition with the specified name cannot be found.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method retrieves the complete definition for a specific setting by its unique name.
        /// The definition contains all metadata about the setting including its default value, scopes,
        /// validation rules, display information, and other configuration details.
        /// </para>
        /// <para>
        /// <strong>Error Handling:</strong> This method throws an exception if the setting definition
        /// cannot be found. This is intentional to ensure that only valid, registered settings are used
        /// throughout the application.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method should be optimized for fast lookup, typically
        /// using a dictionary or similar data structure for O(1) access time.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Validate setting values against their definitions</description></item>
        /// <item><description>Retrieve default values for settings</description></item>
        /// <item><description>Check setting scopes and permissions</description></item>
        /// <item><description>Generate configuration UI elements</description></item>
        /// <item><description>Provide setting metadata for documentation</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        SettingDefinition GetSettingDefinition(string name);

        /// <summary>
        /// Gets a list of all setting definitions registered in the system.
        /// </summary>
        /// <returns>
        /// A read-only list containing all setting definitions in the system.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves all setting definitions that have been registered in the system.
        /// The returned list is read-only to prevent modification of the underlying collection.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method can be expensive as it returns all setting
        /// definitions. Consider caching the result if you need to access this information frequently.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Generate comprehensive configuration documentation</description></item>
        /// <item><description>Build configuration management interfaces</description></item>
        /// <item><description>Perform system-wide setting analysis</description></item>
        /// <item><description>Export setting definitions for backup or migration</description></item>
        /// <item><description>Validate setting consistency across the system</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Ordering:</strong> The order of settings in the returned list is not guaranteed
        /// and may vary between calls. If you need a specific ordering, sort the results based on
        /// your requirements (e.g., by name, group, or scope).
        /// </para>
        /// </remarks>
        IReadOnlyList<SettingDefinition> GetAllSettingDefinitions();
    }
}
