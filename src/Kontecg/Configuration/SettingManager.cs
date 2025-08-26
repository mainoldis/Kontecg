using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Main implementation of <see cref="ISettingManager"/> that manages setting values in the database
    /// with support for multi-level caching and hierarchical setting resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// SettingManager is the core component responsible for managing configuration settings across
    /// different scopes (Application, Company, User) in the Kontecg framework. It provides a robust
    /// implementation with built-in caching, encryption support, and hierarchical setting resolution.
    /// </para>
    /// <para>
    /// <strong>Architecture Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Multi-Level Caching:</strong> Application, company, and user-level caches for optimal performance</description></item>
    /// <item><description><strong>Setting Encryption:</strong> Built-in support for encrypting sensitive setting values</description></item>
    /// <item><description><strong>Hierarchical Resolution:</strong> User → Company → Application → Default value resolution</description></item>
    /// <item><description><strong>Multi-Tenancy Support:</strong> Company-specific settings for multi-tenant applications</description></item>
    /// <item><description><strong>Async Operations:</strong> Full async/await support for non-blocking operations</description></item>
    /// <item><description><strong>Unit of Work Integration:</strong> Proper transaction management for setting changes</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Caching Strategy:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Application Cache:</strong> Caches all application-level settings</description></item>
    /// <item><description><strong>Company Cache:</strong> Caches company-specific settings per company ID</description></item>
    /// <item><description><strong>User Cache:</strong> Caches user-specific settings per user</description></item>
    /// <item><description><strong>Cache Invalidation:</strong> Automatic cache invalidation when settings are modified</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Setting Resolution Logic:</strong>
    /// <list type="number">
    /// <item><description>Check user-specific setting (if user context available)</description></item>
    /// <item><description>Check company-specific setting (if company context available)</description></item>
    /// <item><description>Check application-level setting</description></item>
    /// <item><description>Fall back to default value from setting definition</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Performance Optimizations:</strong>
    /// <list type="bullet">
    /// <item><description>Lazy loading of setting values</description></item>
    /// <item><description>Bulk operations for multiple settings</description></item>
    /// <item><description>Cache warming strategies</description></item>
    /// <item><description>Minimal database round trips</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> This class is thread-safe and can be used concurrently
    /// from multiple threads. All internal operations are properly synchronized.
    /// </para>
    /// <para>
    /// <strong>Dependency Injection:</strong> This class is registered as a singleton in the
    /// dependency injection container to ensure consistent setting management across the application.
    /// </para>
    /// </remarks>
    public class SettingManager : ISettingManager, ISingletonDependency
    {
        /// <summary>
        /// Cache key used for storing application-level settings in the cache.
        /// </summary>
        public const string ApplicationSettingsCacheKey = "ApplicationSettings";
        
        private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _applicationSettingCache;
        private readonly ITypedCache<int, Dictionary<string, SettingInfo>> _companySettingCache;
        private readonly ICompanyStore _companyStore;
        private readonly IMultiCompanyConfig _multiCompanyConfig;

        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _userSettingCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingManager"/> class with required dependencies.
        /// </summary>
        /// <param name="settingDefinitionManager">
        /// The setting definition manager used to retrieve setting definitions and metadata.
        /// </param>
        /// <param name="cacheManager">
        /// The cache manager used to create and manage setting caches at different levels.
        /// </param>
        /// <param name="multiCompanyConfig">
        /// The multi-company configuration that determines company-specific behavior.
        /// </param>
        /// <param name="companyStore">
        /// The company store used to validate and retrieve company information.
        /// </param>
        /// <param name="settingEncryptionService">
        /// The encryption service used to encrypt/decrypt sensitive setting values.
        /// </param>
        /// <param name="unitOfWorkManager">
        /// The unit of work manager used to manage database transactions for setting operations.
        /// </param>
        /// <remarks>
        /// <para>
        /// This constructor initializes the SettingManager with all required dependencies for
        /// managing settings across different scopes with proper caching, encryption, and
        /// transaction management.
        /// </para>
        /// <para>
        /// <strong>Dependencies:</strong>
        /// <list type="bullet">
        /// <item><description>ISettingDefinitionManager: Provides setting definitions and metadata</description></item>
        /// <item><description>ICacheManager: Manages multi-level caching for settings</description></item>
        /// <item><description>IMultiCompanyConfig: Configures multi-tenant behavior</description></item>
        /// <item><description>ICompanyStore: Validates and retrieves company information</description></item>
        /// <item><description>ISettingEncryptionService: Handles encryption of sensitive settings</description></item>
        /// <item><description>IUnitOfWorkManager: Manages database transactions</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public SettingManager(
            ISettingDefinitionManager settingDefinitionManager,
            ICacheManager cacheManager,
            IMultiCompanyConfig multiCompanyConfig,
            ICompanyStore companyStore,
            ISettingEncryptionService settingEncryptionService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _multiCompanyConfig = multiCompanyConfig;
            _companyStore = companyStore;
            SettingEncryptionService = settingEncryptionService;
            _unitOfWorkManager = unitOfWorkManager;

            KontecgSession = NullKontecgSession.Instance;
            SettingStore = DefaultConfigSettingStore.Instance;

            _applicationSettingCache = cacheManager.GetApplicationSettingsCache();
            _companySettingCache = cacheManager.GetCompanySettingsCache();
            _userSettingCache = cacheManager.GetUserSettingsCache();
        }

        /// <summary>
        /// Gets or sets the reference to the current session for user and company context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property provides access to the current session context, which includes
        /// information about the current user and company. This context is used for
        /// setting resolution and determining the appropriate scope for setting operations.
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> Initially set to <see cref="NullKontecgSession.Instance"/>
        /// and should be replaced with the actual session implementation during application startup.
        /// </para>
        /// </remarks>
        public IKontecgSession KontecgSession { get; set; }

        /// <summary>
        /// Gets or sets the reference to the setting store used for persistent storage.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property provides access to the setting store that handles the persistent
        /// storage and retrieval of setting values. The store is responsible for database
        /// operations related to settings.
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> Initially set to <see cref="DefaultConfigSettingStore.Instance"/>
        /// and should be replaced with the appropriate store implementation during application startup.
        /// </para>
        /// </remarks>
        public ISettingStore SettingStore { get; set; }

        /// <summary>
        /// Gets the setting encryption service used for encrypting/decrypting sensitive setting values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property provides access to the encryption service that handles the encryption
        /// and decryption of sensitive setting values. Settings marked as requiring encryption
        /// will be automatically encrypted when stored and decrypted when retrieved.
        /// </para>
        /// </remarks>
        protected ISettingEncryptionService SettingEncryptionService { get; }

        #region Nested classes

        /// <summary>
        /// Internal implementation of <see cref="ISettingValue"/> used for returning setting values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class provides a simple implementation of the ISettingValue interface for
        /// returning setting values from the SettingManager. It encapsulates the setting name
        /// and value in a read-only structure.
        /// </para>
        /// </remarks>
        private class SettingValueObject : ISettingValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SettingValueObject"/> class.
            /// </summary>
            /// <param name="name">The name of the setting.</param>
            /// <param name="value">The value of the setting.</param>
            public SettingValueObject(string name, string value)
            {
                Value = value;
                Name = name;
            }

            /// <summary>
            /// Gets the name of the setting.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the value of the setting.
            /// </summary>
            public string Value { get; }
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value using the current session context, which includes
        /// the current user and company. The setting is resolved through the hierarchical system.
        /// </para>
        /// <para>
        /// <strong>Implementation Details:</strong>
        /// <list type="bullet">
        /// <item><description>Uses the current session to determine user and company context</description></item>
        /// <item><description>Resolves setting through the hierarchy: user → company → application → default</description></item>
        /// <item><description>Applies encryption/decryption if the setting requires it</description></item>
        /// <item><description>Uses caching for optimal performance</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public Task<string> GetSettingValueAsync(string name)
        {
            return GetSettingValueInternalAsync(name, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        /// <inheritdoc/>
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
        public string GetSettingValue(string name)
        {
            return GetSettingValueInternal(name, KontecgSession.CompanyId, KontecgSession.UserId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method retrieves only the application-level setting value, ignoring any company or user
        /// overrides. If no application-level value is set, it returns the default value.
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
        public Task<string> GetSettingValueForApplicationAsync(string name)
        {
            return GetSettingValueInternalAsync(name);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForApplicationAsync(string)"/>. This method
        /// retrieves only the application-level setting value without considering company or user overrides.
        /// </para>
        /// </remarks>
        public string GetSettingValueForApplication(string name)
        {
            return GetSettingValueInternal(name);
        }

        /// <inheritdoc/>
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
        public Task<string> GetSettingValueForApplicationAsync(string name, bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, fallbackToDefault: fallbackToDefault);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForApplicationAsync(string, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        public string GetSettingValueForApplication(string name, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, fallbackToDefault: fallbackToDefault);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value for a specific company, falling back to application-level
        /// settings and then default values if no company-specific setting is found.
        /// </para>
        /// <para>
        /// <strong>Multi-Tenancy:</strong> This method is essential for multi-tenant applications where
        /// different companies may have different configuration requirements.
        /// </para>
        /// </remarks>
        public Task<string> GetSettingValueForCompanyAsync(string name, int companyId)
        {
            return GetSettingValueInternalAsync(name, companyId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForCompanyAsync(string, int)"/>. This method
        /// retrieves company-specific settings but executes synchronously.
        /// </para>
        /// </remarks>
        public string GetSettingValueForCompany(string name, int companyId)
        {
            return GetSettingValueInternal(name, companyId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method provides control over whether to fall back to application-level settings when no
        /// company-specific setting is explicitly set. This is useful for distinguishing between
        /// "not set for company" and "using application default" scenarios.
        /// </para>
        /// </remarks>
        public Task<string> GetSettingValueForCompanyAsync(string name, int companyId, bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, companyId, fallbackToDefault: fallbackToDefault);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForCompanyAsync(string, int, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        public string GetSettingValueForCompany(string name, int companyId, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, companyId, fallbackToDefault: fallbackToDefault);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method retrieves the setting value for a specific user, falling back through the hierarchy:
        /// user → company → application → default.
        /// </para>
        /// <para>
        /// <strong>User Preferences:</strong> This method is essential for user-specific settings like
        /// UI preferences, notification settings, and personal configurations.
        /// </para>
        /// </remarks>
        public Task<string> GetSettingValueForUserAsync(string name, int? companyId, long userId)
        {
            return GetSettingValueInternalAsync(name, companyId, userId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForUserAsync(string, int?, long)"/>. This method
        /// retrieves user-specific settings but executes synchronously.
        /// </para>
        /// </remarks>
        public string GetSettingValueForUser(string name, int? companyId, long userId)
        {
            return GetSettingValueInternal(name, companyId, userId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// This method provides control over whether to fall back to company/application settings when no
        /// user-specific setting is explicitly set. This is useful for distinguishing between
        /// "not set for user" and "using company/application default" scenarios.
        /// </para>
        /// </remarks>
        public Task<string> GetSettingValueForUserAsync(string name, int? companyId, long userId,
            bool fallbackToDefault)
        {
            return GetSettingValueInternalAsync(name, companyId, userId, fallbackToDefault);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetSettingValueForUserAsync(string, int?, long, bool)"/>. This method
        /// provides the same fallback control but executes synchronously.
        /// </para>
        /// </remarks>
        public string GetSettingValueForUser(string name, int? companyId, long userId, bool fallbackToDefault)
        {
            return GetSettingValueInternal(name, companyId, userId, fallbackToDefault);
        }

        /// <inheritdoc/>
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
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync()
        {
            return await GetAllSettingValuesAsync(SettingScopes.Application | SettingScopes.Company |
                                                  SettingScopes.User);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesAsync()"/>. This method retrieves all
        /// settings but executes synchronously.
        /// </para>
        /// </remarks>
        public IReadOnlyList<ISettingValue> GetAllSettingValues()
        {
            return GetAllSettingValues(SettingScopes.Application | SettingScopes.Company | SettingScopes.User);
        }

        /// <inheritdoc/>
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
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync(SettingScopes scopes)
        {
            Dictionary<string, SettingDefinition> settingDefinitions = new Dictionary<string, SettingDefinition>();
            Dictionary<string, ISettingValue> settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (SettingDefinition setting in _settingDefinitionManager.GetAllSettingDefinitions())
            {
                settingDefinitions[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValueObject(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            if (scopes.HasFlag(SettingScopes.Application))
            {
                foreach (ISettingValue settingValue in await GetAllSettingValuesForApplicationAsync())
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Application))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        ((setting.Scopes.HasFlag(SettingScopes.Company) && KontecgSession.CompanyId.HasValue) ||
                         (setting.Scopes.HasFlag(SettingScopes.User) && KontecgSession.UserId.HasValue)))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite company settings
            if (scopes.HasFlag(SettingScopes.Company) && KontecgSession.CompanyId.HasValue)
            {
                foreach (ISettingValue settingValue in await GetAllSettingValuesForCompanyAsync(KontecgSession.CompanyId
                             .Value))
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Company))
                    {
                        continue;
                    }

                    if (!setting.IsInherited && setting.Scopes.HasFlag(SettingScopes.User) &&
                        KontecgSession.UserId.HasValue)
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite user settings
            if (scopes.HasFlag(SettingScopes.User) && KontecgSession.UserId.HasValue)
            {
                foreach (ISettingValue settingValue in await GetAllSettingValuesForUserAsync(
                             KontecgSession.ToUserIdentifier()))
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                    {
                        settingValues[settingValue.Name] =
                            new SettingValueObject(settingValue.Name, settingValue.Value);
                    }
                }
            }

            return settingValues.Values.ToImmutableList();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Synchronous version of <see cref="GetAllSettingValuesAsync(SettingScopes)"/>. This method retrieves
        /// all settings but executes synchronously.
        /// </para>
        /// </remarks>
        public IReadOnlyList<ISettingValue> GetAllSettingValues(SettingScopes scopes)
        {
            Dictionary<string, SettingDefinition> settingDefinitions = new Dictionary<string, SettingDefinition>();
            Dictionary<string, ISettingValue> settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (SettingDefinition setting in _settingDefinitionManager.GetAllSettingDefinitions())
            {
                settingDefinitions[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValueObject(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            if (scopes.HasFlag(SettingScopes.Application))
            {
                foreach (ISettingValue settingValue in GetAllSettingValuesForApplication())
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Application))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        ((setting.Scopes.HasFlag(SettingScopes.Company) && KontecgSession.CompanyId.HasValue) ||
                         (setting.Scopes.HasFlag(SettingScopes.User) && KontecgSession.UserId.HasValue)))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite company settings
            if (scopes.HasFlag(SettingScopes.Company) && KontecgSession.CompanyId.HasValue)
            {
                foreach (ISettingValue settingValue in GetAllSettingValuesForCompany(KontecgSession.CompanyId.Value))
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Company))
                    {
                        continue;
                    }

                    if (!setting.IsInherited && setting.Scopes.HasFlag(SettingScopes.User) &&
                        KontecgSession.UserId.HasValue)
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite user settings
            if (scopes.HasFlag(SettingScopes.User) && KontecgSession.UserId.HasValue)
            {
                foreach (ISettingValue settingValue in GetAllSettingValuesForUser(KontecgSession.ToUserIdentifier()))
                {
                    SettingDefinition setting = settingDefinitions.GetOrDefault(settingValue.Name);
                    if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                    {
                        settingValues[settingValue.Name] =
                            new SettingValueObject(settingValue.Name, settingValue.Value);
                    }
                }
            }

            return settingValues.Values.ToImmutableList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForApplicationAsync()
        {
            if (!_multiCompanyConfig.IsEnabled)
            {
                return (await GetReadOnlyCompanySettingsAsync(KontecgSession.GetCompanyId())).Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList();
            }

            return (await GetApplicationSettingsAsync()).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication()
        {
            if (!_multiCompanyConfig.IsEnabled)
            {
                return GetReadOnlyCompanySettings(KontecgSession.GetCompanyId()).Values
                    .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                    .ToImmutableList();
            }

            return GetApplicationSettings().Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForCompanyAsync(int companyId)
        {
            return (await GetReadOnlyCompanySettingsAsync(companyId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForCompany(int companyId)
        {
            return GetReadOnlyCompanySettings(companyId).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(long userId)
        {
            return GetAllSettingValuesForUserAsync(new UserIdentifier(KontecgSession.CompanyId, userId));
        }

        /// <inheritdoc />
        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(long userId)
        {
            return GetAllSettingValuesForUser(new UserIdentifier(KontecgSession.CompanyId, userId));
        }

        public async Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(UserIdentifier user)
        {
            return (await GetReadOnlyUserSettingsAsync(user)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(UserIdentifier user)
        {
            return GetReadOnlyUserSettings(user).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public virtual async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (_multiCompanyConfig.IsEnabled)
                {
                    await InsertOrUpdateOrDeleteSettingValueAsync(name, value, null, null);
                }
                else
                {
                    // If MultiCompany is disabled, then we should change default company's setting
                    await InsertOrUpdateOrDeleteSettingValueAsync(name, value, KontecgSession.GetCompanyId(), null);
                    await _companySettingCache.RemoveAsync(KontecgSession.GetCompanyId());
                }

                await _applicationSettingCache.RemoveAsync(ApplicationSettingsCacheKey);
            });
        }

        /// <inheritdoc />
        public virtual void ChangeSettingForApplication(string name, string value)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (_multiCompanyConfig.IsEnabled)
                {
                    InsertOrUpdateOrDeleteSettingValue(name, value, null, null);
                }
                else
                {
                    // If MultiCompany is disabled, then we should change default company's setting
                    InsertOrUpdateOrDeleteSettingValue(name, value, KontecgSession.GetCompanyId(), null);
                    _companySettingCache.Remove(KontecgSession.GetCompanyId());
                }

                _applicationSettingCache.Remove(ApplicationSettingsCacheKey);
            });
        }

        /// <inheritdoc />
        public virtual async Task ChangeSettingForCompanyAsync(int companyId, string name, string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await InsertOrUpdateOrDeleteSettingValueAsync(name, value, companyId, null);
                await _companySettingCache.RemoveAsync(companyId);
            });
        }

        /// <inheritdoc />
        public virtual void ChangeSettingForCompany(int companyId, string name, string value)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                InsertOrUpdateOrDeleteSettingValue(name, value, companyId, null);
                _companySettingCache.Remove(companyId);
            });
        }

        public Task ChangeSettingForUserAsync(long userId, string name, string value)
        {
            return ChangeSettingForUserAsync(new UserIdentifier(KontecgSession.CompanyId, userId), name, value);
        }

        public void ChangeSettingForUser(long userId, string name, string value)
        {
            ChangeSettingForUser(new UserIdentifier(KontecgSession.CompanyId, userId), name, value);
        }

        /// <inheritdoc />
        public virtual async Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await InsertOrUpdateOrDeleteSettingValueAsync(name, value, user.CompanyId, user.UserId);
                await _userSettingCache.RemoveAsync(user.ToUserIdentifierString());
            });
        }

        /// <inheritdoc />
        public virtual void ChangeSettingForUser(UserIdentifier user, string name, string value)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                InsertOrUpdateOrDeleteSettingValue(name, value, user.CompanyId, user.UserId);
                _userSettingCache.Remove(user.ToUserIdentifierString());
            });
        }

        #endregion

        #region Private methods

        private async Task<string> GetSettingValueInternalAsync(string name, int? companyId = null, long? userId = null,
            bool fallbackToDefault = true)
        {
            SettingDefinition settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                SettingInfo settingValue =
                    await GetSettingValueForUserOrNullAsync(
                        new UserIdentifier(companyId, userId.Value),
                        name
                    );

                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for company if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Company) && companyId.HasValue)
            {
                SettingInfo settingValue = await GetSettingValueForCompanyOrNullAsync(companyId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for application if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Application))
            {
                SettingInfo settingValue = await GetSettingValueForApplicationOrNullAsync(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }
            }

            //Not defined, get default value
            return settingDefinition.DefaultValue;
        }

        private string GetSettingValueInternal(string name, int? companyId = null, long? userId = null,
            bool fallbackToDefault = true)
        {
            SettingDefinition settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);

            //Get for user if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                SettingInfo settingValue =
                    GetSettingValueForUserOrNull(new UserIdentifier(companyId, userId.Value), name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for company if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Company) && companyId.HasValue)
            {
                SettingInfo settingValue = GetSettingValueForCompanyOrNull(companyId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.IsInherited)
                {
                    return settingDefinition.DefaultValue;
                }
            }

            //Get for application if defined
            if (settingDefinition.Scopes.HasFlag(SettingScopes.Application))
            {
                SettingInfo settingValue = GetSettingValueForApplicationOrNull(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }
            }

            //Not defined, get default value
            return settingDefinition.DefaultValue;
        }

        private async Task<SettingInfo> InsertOrUpdateOrDeleteSettingValueAsync(string name, string value,
            int? companyId, long? userId)
        {
            SettingDefinition settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            SettingInfo settingValue = await SettingStore.GetSettingOrNullAsync(companyId, userId, name);

            //Determine defaultValue
            string defaultValue = settingDefinition.DefaultValue;

            if (settingDefinition.IsInherited)
            {
                //For Company and User, Application's value overrides Setting Definition's default value when multi company is enabled.
                if (_multiCompanyConfig.IsEnabled && (companyId.HasValue || userId.HasValue))
                {
                    SettingInfo applicationValue = await GetSettingValueForApplicationOrNullAsync(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Companys's value overrides Application's default value.
                if (userId.HasValue && companyId.HasValue)
                {
                    SettingInfo companyValue = await GetSettingValueForCompanyOrNullAsync(companyId.Value, name);
                    if (companyValue != null)
                    {
                        defaultValue = companyValue.Value;
                    }
                }
            }

            //No need to store on database if the value is the default value
            if (value == defaultValue)
            {
                if (settingValue != null)
                {
                    await SettingStore.DeleteAsync(settingValue);
                }

                return null;
            }

            //If it's not default value and not stored on database, then insert it
            if (settingValue == null)
            {
                settingValue = new SettingInfo
                {
                    CompanyId = companyId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                if (settingDefinition.IsEncrypted)
                {
                    settingValue.Value = SettingEncryptionService.Encrypt(settingDefinition, value);
                }

                await SettingStore.CreateAsync(settingValue);
                return settingValue;
            }

            //It's same value in database, no need to update
            string rawSettingValue = settingDefinition.IsEncrypted
                ? SettingEncryptionService.Decrypt(settingDefinition, settingValue.Value)
                : settingValue.Value;
            if (rawSettingValue == value)
            {
                return settingValue;
            }

            //Update the setting on database.
            settingValue.Value = settingDefinition.IsEncrypted
                ? SettingEncryptionService.Encrypt(settingDefinition, value)
                : value;
            await SettingStore.UpdateAsync(settingValue);

            return settingValue;
        }

        private SettingInfo InsertOrUpdateOrDeleteSettingValue(string name, string value, int? companyId, long? userId)
        {
            SettingDefinition settingDefinition = _settingDefinitionManager.GetSettingDefinition(name);
            SettingInfo settingValue = SettingStore.GetSettingOrNull(companyId, userId, name);

            //Determine defaultValue
            string defaultValue = settingDefinition.DefaultValue;

            if (settingDefinition.IsInherited)
            {
                //For Company and User, Application's value overrides Setting Definition's default value when multi company is enabled.
                if (_multiCompanyConfig.IsEnabled && (companyId.HasValue || userId.HasValue))
                {
                    SettingInfo applicationValue = GetSettingValueForApplicationOrNull(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Companys's value overrides Application's default value.
                if (userId.HasValue && companyId.HasValue)
                {
                    SettingInfo companyValue = GetSettingValueForCompanyOrNull(companyId.Value, name);
                    if (companyValue != null)
                    {
                        defaultValue = companyValue.Value;
                    }
                }
            }

            //No need to store on database if the value is the default value
            if (value == defaultValue)
            {
                if (settingValue != null)
                {
                    SettingStore.Delete(settingValue);
                }

                return null;
            }

            //If it's not default value and not stored on database, then insert it
            if (settingValue == null)
            {
                settingValue = new SettingInfo
                {
                    CompanyId = companyId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                if (settingDefinition.IsEncrypted)
                {
                    settingValue.Value = SettingEncryptionService.Encrypt(settingDefinition, value);
                }

                SettingStore.Create(settingValue);
                return settingValue;
            }

            string rawSettingValue = settingDefinition.IsEncrypted
                ? SettingEncryptionService.Decrypt(settingDefinition, settingValue.Value)
                : settingValue.Value;
            if (rawSettingValue == value)
            {
                return settingValue;
            }

            //Update the setting on database.
            settingValue.Value = settingDefinition.IsEncrypted
                ? SettingEncryptionService.Encrypt(settingDefinition, value)
                : value;
            SettingStore.Update(settingValue);

            return settingValue;
        }

        private async Task<SettingInfo> GetSettingValueForApplicationOrNullAsync(string name)
        {
            if (_multiCompanyConfig.IsEnabled)
            {
                return (await GetApplicationSettingsAsync()).GetOrDefault(name);
            }

            return (await GetReadOnlyCompanySettingsAsync(KontecgSession.GetCompanyId())).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForApplicationOrNull(string name)
        {
            if (_multiCompanyConfig.IsEnabled)
            {
                return GetApplicationSettings().GetOrDefault(name);
            }

            return GetReadOnlyCompanySettings(KontecgSession.GetCompanyId()).GetOrDefault(name);
        }

        private async Task<SettingInfo> GetSettingValueForCompanyOrNullAsync(int companyId, string name)
        {
            return (await GetReadOnlyCompanySettingsAsync(companyId)).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForCompanyOrNull(int companyId, string name)
        {
            return GetReadOnlyCompanySettings(companyId).GetOrDefault(name);
        }

        private async Task<SettingInfo> GetSettingValueForUserOrNullAsync(UserIdentifier user, string name)
        {
            return (await GetReadOnlyUserSettingsAsync(user)).GetOrDefault(name);
        }

        private SettingInfo GetSettingValueForUserOrNull(UserIdentifier user, string name)
        {
            return GetReadOnlyUserSettings(user).GetOrDefault(name);
        }

        private async Task<Dictionary<string, SettingInfo>> GetApplicationSettingsAsync()
        {
            return await _applicationSettingCache.GetAsync(ApplicationSettingsCacheKey, async () =>
            {
                List<SettingInfo> settingValues = await SettingStore.GetAllListAsync(null, null);
                return ConvertSettingInfosToDictionary(settingValues);
            });
        }

        private Dictionary<string, SettingInfo> GetApplicationSettings()
        {
            return _applicationSettingCache.Get(ApplicationSettingsCacheKey, () =>
            {
                List<SettingInfo> settingValues = SettingStore.GetAllList(null, null);
                return ConvertSettingInfosToDictionary(settingValues);
            });
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyCompanySettingsAsync(int companyId)
        {
            Dictionary<string, SettingInfo> cachedDictionary = await GetCompanySettingsFromCacheAsync(companyId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private ImmutableDictionary<string, SettingInfo> GetReadOnlyCompanySettings(int companyId)
        {
            Dictionary<string, SettingInfo> cachedDictionary = GetCompanySettingsFromCache(companyId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyUserSettingsAsync(UserIdentifier user)
        {
            Dictionary<string, SettingInfo> cachedDictionary = await GetUserSettingsFromCacheAsync(user);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private ImmutableDictionary<string, SettingInfo> GetReadOnlyUserSettings(UserIdentifier user)
        {
            Dictionary<string, SettingInfo> cachedDictionary = GetUserSettingsFromCache(user);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<Dictionary<string, SettingInfo>> GetCompanySettingsFromCacheAsync(int companyId)
        {
            return await _companySettingCache.GetAsync(
                companyId,
                async () =>
                {
                    if (!_multiCompanyConfig.IsEnabled && _companyStore.Find(companyId) == null)
                    {
                        return new Dictionary<string, SettingInfo>();
                    }

                    List<SettingInfo> settingValues = await SettingStore.GetAllListAsync(companyId, null);
                    return ConvertSettingInfosToDictionary(settingValues);
                });
        }

        private Dictionary<string, SettingInfo> GetCompanySettingsFromCache(int companyId)
        {
            return _companySettingCache.Get(
                companyId,
                () =>
                {
                    if (!_multiCompanyConfig.IsEnabled && _companyStore.Find(companyId) == null)
                    {
                        return new Dictionary<string, SettingInfo>();
                    }

                    List<SettingInfo> settingValues = SettingStore.GetAllList(companyId, null);
                    return ConvertSettingInfosToDictionary(settingValues);
                });
        }

        private async Task<Dictionary<string, SettingInfo>> GetUserSettingsFromCacheAsync(UserIdentifier user)
        {
            return await _userSettingCache.GetAsync(
                user.ToUserIdentifierString(),
                async () =>
                {
                    List<SettingInfo> settingValues = await SettingStore.GetAllListAsync(user.CompanyId, user.UserId);
                    return ConvertSettingInfosToDictionary(settingValues);
                });
        }

        private Dictionary<string, SettingInfo> ConvertSettingInfosToDictionary(List<SettingInfo> settingValues)
        {
            Dictionary<string, SettingInfo> dictionary = new Dictionary<string, SettingInfo>();
            IReadOnlyList<SettingDefinition> allSettingDefinitions =
                _settingDefinitionManager.GetAllSettingDefinitions();

            foreach (var setting in allSettingDefinitions.Join(settingValues,
                         definition => definition.Name,
                         value => value.Name,
                         (definition, value) => new
                         {
                             SettingDefinition = definition,
                             SettingValue = value
                         }))
            {
                if (setting.SettingDefinition.IsEncrypted)
                {
                    setting.SettingValue.Value =
                        SettingEncryptionService.Decrypt(setting.SettingDefinition, setting.SettingValue.Value);
                }

                dictionary[setting.SettingValue.Name] = setting.SettingValue;
            }

            return dictionary;
        }

        private Dictionary<string, SettingInfo> GetUserSettingsFromCache(UserIdentifier user)
        {
            return _userSettingCache.Get(
                user.ToUserIdentifierString(),
                () =>
                {
                    List<SettingInfo> settingValues = SettingStore.GetAllList(user.CompanyId, user.UserId);
                    return ConvertSettingInfosToDictionary(settingValues);
                });
        }

        public Task<string> GetSettingValueForUserAsync(string name, UserIdentifier user)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(user, nameof(user));

            return GetSettingValueForUserAsync(name, user.CompanyId, user.UserId);
        }

        public string GetSettingValueForUser(string name, UserIdentifier user)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(user, nameof(user));

            return GetSettingValueForUser(name, user.CompanyId, user.UserId);
        }

        #endregion
    }
}
