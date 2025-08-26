using System;
using Kontecg.Localization;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines a configuration setting within the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// SettingDefinition represents a configuration setting that can be used to configure
    /// and change the behavior of the application. Each setting has a unique name, default
    /// value, and various metadata that controls how it behaves and is displayed.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Unique Identification:</strong> Each setting has a unique name for identification</description></item>
    /// <item><description><strong>Default Values:</strong> Settings can have default values that are used when no explicit value is set</description></item>
    /// <item><description><strong>Localization Support:</strong> Display names and descriptions can be localized</description></item>
    /// <item><description><strong>Scoping:</strong> Settings can be scoped to different levels (Application, Tenant, User)</description></item>
    /// <item><description><strong>Inheritance:</strong> Settings can inherit values from parent scopes</description></item>
    /// <item><description><strong>Client Visibility:</strong> Control whether settings are visible to client applications</description></item>
    /// <item><description><strong>Encryption:</strong> Settings can be stored encrypted for security</description></item>
    /// <item><description><strong>Grouping:</strong> Settings can be organized into logical groups</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Setting Scopes:</strong> Settings can be scoped to different levels:
    /// <list type="bullet">
    /// <item><description><strong>Application:</strong> Global settings that apply to the entire application</description></item>
    /// <item><description><strong>Tenant:</strong> Settings specific to a tenant in multi-tenant applications</description></item>
    /// <item><description><strong>User:</strong> Settings specific to individual users</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Client Visibility:</strong> The framework provides different visibility providers:
    /// <list type="bullet">
    /// <item><description><strong>Hidden:</strong> Settings not visible to client applications</description></item>
    /// <item><description><strong>Visible:</strong> Settings visible to all client applications</description></item>
    /// <item><description><strong>Requires Authentication:</strong> Settings visible only to authenticated clients</description></item>
    /// <item><description><strong>Requires Permission:</strong> Settings visible only to clients with specific permissions</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class SettingDefinition
    {
        /// <summary>
        /// Creates a new <see cref="SettingDefinition"/> object with the specified parameters.
        /// </summary>
        /// <param name="name">
        /// The unique name of the setting. This name is used to identify and retrieve the setting.
        /// Must not be null or empty.
        /// </param>
        /// <param name="defaultValue">
        /// The default value of the setting. This value is used when no explicit value
        /// has been set for the setting.
        /// </param>
        /// <param name="displayName">
        /// The display name of the setting. This can be used to show the setting to users
        /// in configuration interfaces. Can be localized.
        /// </param>
        /// <param name="group">
        /// The group that this setting belongs to. Used for organizing settings into
        /// logical categories.
        /// </param>
        /// <param name="description">
        /// A brief description of the setting. Provides additional context about what
        /// the setting controls. Can be localized.
        /// </param>
        /// <param name="scopes">
        /// The scopes where this setting can be used. Default value is
        /// <see cref="SettingScopes.Application"/>.
        /// </param>
        /// <param name="isVisibleToClients">
        /// This parameter is obsolete. Use <paramref name="clientVisibilityProvider"/> instead!
        /// Default value is false.
        /// </param>
        /// <param name="isInherited">
        /// Indicates whether this setting inherits values from parent scopes.
        /// Default value is true.
        /// </param>
        /// <param name="customData">
        /// Can be used to store custom data related to this setting. This allows for
        /// extensibility and additional metadata.
        /// </param>
        /// <param name="clientVisibilityProvider">
        /// Defines the client visibility rules for this setting. Controls whether and
        /// how the setting is visible to client applications. Default is invisible.
        /// </param>
        /// <param name="isEncrypted">
        /// Indicates whether this setting should be stored encrypted in the data source.
        /// Default value is false.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the name parameter is null or empty.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This constructor creates a new setting definition with all the necessary
        /// metadata to configure how the setting behaves within the framework.
        /// </para>
        /// <para>
        /// <strong>Client Visibility:</strong> The client visibility is determined by the
        /// following priority order:
        /// <list type="number">
        /// <item><description>If <paramref name="clientVisibilityProvider"/> is provided, it is used</description></item>
        /// <item><description>If <paramref name="isVisibleToClients"/> is true, a visible provider is used</description></item>
        /// <item><description>Otherwise, a hidden provider is used by default</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The constructor validates that the name parameter
        /// is not null or empty, as the name is essential for setting identification.
        /// </para>
        /// <para>
        /// <strong>Default Behavior:</strong> By default, settings are:
        /// <list type="bullet">
        /// <item><description>Scoped to Application level</description></item>
        /// <item><description>Inherited from parent scopes</description></item>
        /// <item><description>Hidden from client applications</description></item>
        /// <item><description>Not encrypted</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public SettingDefinition(
            string name,
            string defaultValue,
            ILocalizableString displayName = null,
            SettingDefinitionGroup group = null,
            ILocalizableString description = null,
            SettingScopes scopes = SettingScopes.Application,
            bool isVisibleToClients = false,
            bool isInherited = true,
            object customData = null,
            ISettingClientVisibilityProvider clientVisibilityProvider = null,
            bool isEncrypted = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            DefaultValue = defaultValue;
            DisplayName = displayName;
            Group = group;
            Description = description;
            Scopes = scopes;
            IsInherited = isInherited;
            CustomData = customData;
            IsEncrypted = isEncrypted;

            ClientVisibilityProvider = new HiddenSettingClientVisibilityProvider();

            if (isVisibleToClients)
            {
                ClientVisibilityProvider = new VisibleSettingClientVisibilityProvider();
            }
            else if (clientVisibilityProvider != null)
            {
                ClientVisibilityProvider = clientVisibilityProvider;
            }
        }

        /// <summary>
        /// Gets the unique name of the setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The name is used to identify and retrieve the setting throughout the application.
        /// It must be unique within the scope of the setting system.
        /// </para>
        /// <para>
        /// <strong>Naming Conventions:</strong> It is recommended to use dot notation
        /// for hierarchical organization, such as "Email.Smtp.Host" or "Database.ConnectionString".
        /// </para>
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the display name of the setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The display name is used to show the setting to users in configuration
        /// interfaces and administration panels. It can be localized to support
        /// multiple languages.
        /// </para>
        /// <para>
        /// <strong>Localization:</strong> If the display name implements
        /// <see cref="ILocalizableString"/>, it will be automatically localized
        /// based on the current culture.
        /// </para>
        /// </remarks>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a brief description for this setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The description provides additional context about what the setting controls
        /// and how it affects the application behavior. It can be localized to support
        /// multiple languages.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Descriptions are particularly useful for:
        /// <list type="bullet">
        /// <item><description>Administration interfaces</description></item>
        /// <item><description>Configuration documentation</description></item>
        /// <item><description>Help systems</description></item>
        /// <item><description>API documentation</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// Gets or sets the scopes where this setting can be used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The scopes determine at what levels the setting can be configured and
        /// how it inherits values from parent scopes.
        /// </para>
        /// <para>
        /// <strong>Scope Hierarchy:</strong> Settings follow a hierarchical scope system:
        /// <list type="bullet">
        /// <item><description><strong>Application:</strong> Global settings that apply to the entire application</description></item>
        /// <item><description><strong>Tenant:</strong> Settings specific to a tenant (in multi-tenant applications)</description></item>
        /// <item><description><strong>User:</strong> Settings specific to individual users</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> The default scope is
        /// <see cref="SettingScopes.Application"/>.
        /// </para>
        /// </remarks>
        public SettingScopes Scopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this setting inherits values from parent scopes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When inheritance is enabled, the setting will inherit values from parent scopes
        /// if no explicit value is set at the current scope level.
        /// </para>
        /// <para>
        /// <strong>Inheritance Chain:</strong> The inheritance follows this order:
        /// <list type="number">
        /// <item><description>User scope (if applicable)</description></item>
        /// <item><description>Tenant scope (if applicable)</description></item>
        /// <item><description>Application scope</description></item>
        /// <item><description>Default value</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> The default value is true, meaning settings
        /// inherit values by default.
        /// </para>
        /// </remarks>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets or sets the group for this setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The group is used to organize settings into logical categories. This is
        /// particularly useful for administration interfaces where settings are
        /// displayed in groups.
        /// </para>
        /// <para>
        /// <strong>Grouping Benefits:</strong>
        /// <list type="bullet">
        /// <item><description>Improved organization in configuration interfaces</description></item>
        /// <item><description>Logical separation of related settings</description></item>
        /// <item><description>Easier navigation and management</description></item>
        /// <item><description>Better user experience</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public SettingDefinitionGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the default value of the setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is used when no explicit value has been set for the setting
        /// at any scope level. This provides a fallback value to ensure the application
        /// has a sensible configuration even when settings are not explicitly configured.
        /// </para>
        /// <para>
        /// <strong>Default Value Usage:</strong> The default value is used in the following scenarios:
        /// <list type="bullet">
        /// <item><description>When the setting has never been set</description></item>
        /// <item><description>When the setting value has been cleared</description></item>
        /// <item><description>When the setting is inherited but no parent scope has a value</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the client visibility definition for the setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The client visibility provider controls whether and how the setting is visible
        /// to client applications. This is important for security and access control.
        /// </para>
        /// <para>
        /// <strong>Visibility Providers:</strong> The framework provides several built-in providers:
        /// <list type="bullet">
        /// <item><description><strong>HiddenSettingClientVisibilityProvider:</strong> Settings are not visible to clients</description></item>
        /// <item><description><strong>VisibleSettingClientVisibilityProvider:</strong> Settings are visible to all clients</description></item>
        /// <item><description><strong>RequiresAuthenticationSettingClientVisibilityProvider:</strong> Settings are visible only to authenticated clients</description></item>
        /// <item><description><strong>RequiresPermissionSettingClientVisibilityProvider:</strong> Settings are visible only to clients with specific permissions</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Custom Providers:</strong> You can implement custom visibility providers
        /// by implementing <see cref="ISettingClientVisibilityProvider"/> for more complex
        /// visibility rules.
        /// </para>
        /// </remarks>
        public ISettingClientVisibilityProvider ClientVisibilityProvider { get; set; }

        /// <summary>
        /// Gets or sets custom data related to this setting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Custom data allows for extensibility and additional metadata to be associated
        /// with the setting. This can be used to store any object that provides additional
        /// context or behavior for the setting.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Custom data can be used for:
        /// <list type="bullet">
        /// <item><description>Validation rules</description></item>
        /// <item><description>UI hints and metadata</description></item>
        /// <item><description>Custom behavior implementations</description></item>
        /// <item><description>Integration with external systems</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public object CustomData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this setting is stored as encrypted in the data source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When encryption is enabled, the setting value will be encrypted before being
        /// stored in the data source and decrypted when retrieved. This is useful for
        /// sensitive settings such as connection strings, API keys, or passwords.
        /// </para>
        /// <para>
        /// <strong>Security Considerations:</strong>
        /// <list type="bullet">
        /// <item><description>Encryption adds overhead to setting operations</description></item>
        /// <item><description>Encrypted settings require proper key management</description></item>
        /// <item><description>Only use encryption for truly sensitive data</description></item>
        /// <item><description>Consider the performance impact in high-frequency scenarios</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> The default value is false, meaning settings
        /// are not encrypted by default.
        /// </para>
        /// </remarks>
        public bool IsEncrypted { get; set; }
    }
}
