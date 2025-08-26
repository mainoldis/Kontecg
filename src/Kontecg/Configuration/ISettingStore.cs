using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the contract for persistent storage and retrieval of configuration settings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ISettingStore is the abstraction layer that defines how configuration settings are
    /// persisted and retrieved from a data source (typically a database). It provides a
    /// clean separation between the setting management logic and the actual storage mechanism.
    /// </para>
    /// <para>
    /// <strong>Key Responsibilities:</strong>
    /// <list type="bullet">
    /// <item><description><strong>CRUD Operations:</strong> Create, Read, Update, Delete setting values</description></item>
    /// <item><description><strong>Scope Support:</strong> Handle application, company, and user-level settings</description></item>
    /// <item><description><strong>Bulk Operations:</strong> Retrieve multiple settings efficiently</description></item>
    /// <item><description><strong>Async Support:</strong> Non-blocking operations for better performance</description></item>
    /// <item><description><strong>Data Integrity:</strong> Ensure consistent setting storage and retrieval</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Setting Scope Handling:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Application Level:</strong> companyId = null, userId = null</description></item>
    /// <item><description><strong>Company Level:</strong> companyId = value, userId = null</description></item>
    /// <item><description><strong>User Level:</strong> companyId = value, userId = value</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Implementations should handle null values for companyId and userId appropriately</description></item>
    /// <item><description>Database operations should be optimized for the expected query patterns</description></item>
    /// <item><description>Consider indexing strategies for efficient setting retrieval</description></item>
    /// <item><description>Implement proper error handling and logging</description></item>
    /// <item><description>Support transaction management for data consistency</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Common Implementations:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Database Setting Store:</strong> Stores settings in database tables</description></item>
    /// <item><description><strong>File Setting Store:</strong> Stores settings in configuration files</description></item>
    /// <item><description><strong>Memory Setting Store:</strong> Stores settings in memory (for testing)</description></item>
    /// <item><description><strong>Hybrid Setting Store:</strong> Combines multiple storage mechanisms</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> Implementations should be thread-safe and support
    /// concurrent access from multiple threads. Proper synchronization mechanisms should
    /// be used to ensure data consistency.
    /// </para>
    /// </remarks>
    public interface ISettingStore
    {
        /// <summary>
        /// Gets a setting by its scope and name, or returns null if not found.
        /// </summary>
        /// <param name="companyId">
        /// The company ID for company or user-level settings, or null for application-level settings.
        /// </param>
        /// <param name="userId">
        /// The user ID for user-level settings, or null for application or company-level settings.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to retrieve.
        /// </param>
        /// <returns>
        /// The setting information if found, or null if the setting doesn't exist for the specified scope.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves a specific setting based on the provided scope parameters and setting name.
        /// The scope is determined by the combination of companyId and userId parameters.
        /// </para>
        /// <para>
        /// <strong>Scope Resolution:</strong>
        /// <list type="bullet">
        /// <item><description>companyId = null, userId = null: Application-level setting</description></item>
        /// <item><description>companyId = value, userId = null: Company-level setting</description></item>
        /// <item><description>companyId = value, userId = value: User-level setting</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method should be optimized for single setting retrieval,
        /// typically using indexed database queries for efficient lookup.
        /// </para>
        /// </remarks>
        Task<SettingInfo> GetSettingOrNullAsync(int? companyId, long? userId, string name);

        /// <summary>
        /// Gets a setting by its scope and name, or returns null if not found (synchronous version).
        /// </summary>
        /// <param name="companyId">
        /// The company ID for company or user-level settings, or null for application-level settings.
        /// </param>
        /// <param name="userId">
        /// The user ID for user-level settings, or null for application or company-level settings.
        /// </param>
        /// <param name="name">
        /// The unique name of the setting to retrieve.
        /// </param>
        /// <returns>
        /// The setting information if found, or null if the setting doesn't exist for the specified scope.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingOrNullAsync(int?, long?, string)"/>. This method
        /// performs the same operation but blocks the calling thread until completion.
        /// </para>
        /// <para>
        /// <strong>Performance Note:</strong> Use the async version for better performance in web applications
        /// and other scenarios where blocking operations should be avoided.
        /// </para>
        /// </remarks>
        SettingInfo GetSettingOrNull(int? companyId, long? userId, string name);

        /// <summary>
        /// Deletes a setting from the persistent storage.
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be deleted from storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method removes a setting from the persistent storage. The setting is identified
        /// by its scope (companyId, userId) and name combination.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>If the setting doesn't exist, the operation should complete successfully</description></item>
        /// <item><description>If the setting exists, it should be completely removed from storage</description></item>
        /// <item><description>Related cache entries should be invalidated after deletion</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Transaction Support:</strong> This operation should be part of a transaction
        /// if called within a unit of work context to ensure data consistency.
        /// </para>
        /// </remarks>
        Task DeleteAsync(SettingInfo setting);

        /// <summary>
        /// Deletes a setting from the persistent storage (synchronous version).
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be deleted from storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="DeleteAsync(SettingInfo)"/>. This method performs
        /// the same deletion operation but blocks the calling thread until completion.
        /// </para>
        /// </remarks>
        void Delete(SettingInfo setting);

        /// <summary>
        /// Creates a new setting in the persistent storage.
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be created in storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method adds a new setting to the persistent storage. The setting should not
        /// already exist for the specified scope and name combination.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong>
        /// <list type="bullet">
        /// <item><description>Ensure the setting doesn't already exist for the same scope and name</description></item>
        /// <item><description>Validate the setting value according to its definition</description></item>
        /// <item><description>Apply any required encryption to sensitive setting values</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Transaction Support:</strong> This operation should be part of a transaction
        /// if called within a unit of work context to ensure data consistency.
        /// </para>
        /// <para>
        /// <strong>Cache Invalidation:</strong> After creating a setting, related cache entries
        /// should be invalidated to ensure consistency.
        /// </para>
        /// </remarks>
        Task CreateAsync(SettingInfo setting);

        /// <summary>
        /// Creates a new setting in the persistent storage (synchronous version).
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be created in storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="CreateAsync(SettingInfo)"/>. This method performs
        /// the same creation operation but blocks the calling thread until completion.
        /// </para>
        /// </remarks>
        void Create(SettingInfo setting);

        /// <summary>
        /// Updates an existing setting in the persistent storage.
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be updated in storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method updates an existing setting in the persistent storage. The setting
        /// should already exist for the specified scope and name combination.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong>
        /// <list type="bullet">
        /// <item><description>Ensure the setting exists for the specified scope and name</description></item>
        /// <item><description>Validate the new setting value according to its definition</description></item>
        /// <item><description>Apply any required encryption to sensitive setting values</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Transaction Support:</strong> This operation should be part of a transaction
        /// if called within a unit of work context to ensure data consistency.
        /// </para>
        /// <para>
        /// <strong>Cache Invalidation:</strong> After updating a setting, related cache entries
        /// should be invalidated to ensure consistency.
        /// </para>
        /// </remarks>
        Task UpdateAsync(SettingInfo setting);

        /// <summary>
        /// Updates an existing setting in the persistent storage (synchronous version).
        /// </summary>
        /// <param name="setting">
        /// The setting information object to be updated in storage.
        /// </param>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="UpdateAsync(SettingInfo)"/>. This method performs
        /// the same update operation but blocks the calling thread until completion.
        /// </para>
        /// </remarks>
        void Update(SettingInfo setting);

        /// <summary>
        /// Gets all settings for a specific scope.
        /// </summary>
        /// <param name="companyId">
        /// The company ID for company or user-level settings, or null for application-level settings.
        /// </param>
        /// <param name="userId">
        /// The user ID for user-level settings, or null for application or company-level settings.
        /// </param>
        /// <returns>
        /// A list of all settings that exist for the specified scope.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves all settings for a specific scope. The scope is determined
        /// by the combination of companyId and userId parameters.
        /// </para>
        /// <para>
        /// <strong>Scope Resolution:</strong>
        /// <list type="bullet">
        /// <item><description>companyId = null, userId = null: All application-level settings</description></item>
        /// <item><description>companyId = value, userId = null: All company-level settings for the company</description></item>
        /// <item><description>companyId = value, userId = value: All user-level settings for the user</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method can be expensive as it retrieves all settings
        /// for a scope. Consider implementing pagination or filtering if dealing with large numbers
        /// of settings.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Configuration management UI</description></item>
        /// <item><description>Setting export/backup functionality</description></item>
        /// <item><description>Bulk setting operations</description></item>
        /// <item><description>System diagnostics and troubleshooting</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        Task<List<SettingInfo>> GetAllListAsync(int? companyId, long? userId);

        /// <summary>
        /// Gets all settings for a specific scope (synchronous version).
        /// </summary>
        /// <param name="companyId">
        /// The company ID for company or user-level settings, or null for application-level settings.
        /// </param>
        /// <param name="userId">
        /// The user ID for user-level settings, or null for application or company-level settings.
        /// </param>
        /// <returns>
        /// A list of all settings that exist for the specified scope.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllListAsync(int?, long?)"/>. This method performs
        /// the same bulk retrieval operation but blocks the calling thread until completion.
        /// </para>
        /// <para>
        /// <strong>Performance Note:</strong> Use the async version for better performance in web applications
        /// and other scenarios where blocking operations should be avoided.
        /// </para>
        /// </remarks>
        List<SettingInfo> GetAllList(int? companyId, long? userId);
    }
}
