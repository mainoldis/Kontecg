using System.Globalization;
using Castle.Core.Logging;
using Kontecg.Configuration;
using Kontecg.Domain.Uow;
using Kontecg.Localization;
using Kontecg.Localization.Sources;
using Kontecg.ObjectMapping;

namespace Kontecg
{
    /// <summary>
    /// Base class for all services in the Kontecg framework that provides common functionality
    /// and dependency injection support for essential framework services.
    /// </summary>
    /// <remarks>
    /// <para>
    /// KontecgServiceBase serves as the foundation for all service classes in the Kontecg framework.
    /// It provides property-injected access to commonly used framework services and utility methods
    /// that most services require. This base class implements the Template Method pattern to ensure
    /// consistent service behavior across the application.
    /// </para>
    /// <para>
    /// The class provides access to the following core services:
    /// </para>
    /// <list type="bullet">
    /// <item><description><strong>Unit of Work Management:</strong> For database transaction management</description></item>
    /// <item><description><strong>Localization:</strong> For multi-language string support</description></item>
    /// <item><description><strong>Logging:</strong> For application logging and debugging</description></item>
    /// <item><description><strong>Object Mapping:</strong> For object-to-object transformations</description></item>
    /// <item><description><strong>Settings:</strong> For configuration management</description></item>
    /// </list>
    /// <para>
    /// <strong>Dependency Injection:</strong> All service dependencies are injected through properties,
    /// allowing for flexible configuration and testing. The class uses null object patterns for
    /// optional dependencies to prevent null reference exceptions.
    /// </para>
    /// <para>
    /// <strong>Localization Support:</strong> The class provides convenient methods for accessing
    /// localized strings with support for culture-specific formatting and parameter substitution.
    /// </para>
    /// </remarks>
    public abstract class KontecgServiceBase
    {
        /// <summary>
        /// The localization source instance used for string localization.
        /// </summary>
        /// <remarks>
        /// This field is lazily initialized when the LocalizationSource property is first accessed.
        /// It caches the localization source to improve performance for repeated access.
        /// </remarks>
        private ILocalizationSource _localizationSource;

        /// <summary>
        /// The unit of work manager instance for transaction management.
        /// </summary>
        /// <remarks>
        /// This field stores the injected unit of work manager. It is validated on access
        /// to ensure it has been properly configured before use.
        /// </remarks>
        private IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="KontecgServiceBase"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The constructor initializes all service dependencies with null object implementations
        /// to prevent null reference exceptions. These dependencies should be properly injected
        /// by the dependency injection container before the service is used.
        /// </para>
        /// <para>
        /// <strong>Default Dependencies:</strong>
        /// <list type="bullet">
        /// <item><description>Logger: <see cref="NullLogger.Instance"/></description></item>
        /// <item><description>ObjectMapper: <see cref="NullObjectMapper.Instance"/></description></item>
        /// <item><description>LocalizationManager: <see cref="NullLocalizationManager.Instance"/></description></item>
        /// </list>
        /// </para>
        /// </remarks>
        protected KontecgServiceBase()
        {
            Logger = NullLogger.Instance;
            ObjectMapper = NullObjectMapper.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <summary>
        /// Gets or sets the setting manager for configuration management.
        /// </summary>
        /// <value>
        /// The setting manager instance that provides access to application configuration.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides access to the application's configuration system, allowing
        /// services to read and modify settings at runtime. The setting manager supports
        /// hierarchical configuration with scoping capabilities.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to access application settings, user preferences,
        /// and other configuration values that may change during runtime.
        /// </para>
        /// </remarks>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Gets or sets the unit of work manager for database transaction management.
        /// </summary>
        /// <value>
        /// The unit of work manager instance that controls database transactions.
        /// </value>
        /// <exception cref="KontecgException">
        /// Thrown when attempting to access the UnitOfWorkManager before it has been properly set.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This property provides access to the unit of work manager, which is responsible for
        /// managing database transactions and ensuring data consistency. The unit of work pattern
        /// ensures that all database operations within a single unit of work are either committed
        /// together or rolled back together.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The getter validates that the unit of work manager has been
        /// properly configured before allowing access. This prevents runtime errors due to
        /// uninitialized dependencies.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to begin, commit, or rollback database transactions,
        /// and to access the current active unit of work.
        /// </para>
        /// </remarks>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get => _unitOfWorkManager ?? throw new KontecgException("Must set UnitOfWorkManager before use it.");
            set => _unitOfWorkManager = value;
        }

        /// <summary>
        /// Gets or sets the localization manager for multi-language support.
        /// </summary>
        /// <value>
        /// The localization manager instance that provides access to localized resources.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides access to the localization system, which manages multi-language
        /// support throughout the application. The localization manager handles loading and caching
        /// of localized strings from various sources (XML files, databases, etc.).
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> Initialized with <see cref="NullLocalizationManager.Instance"/>
        /// to prevent null reference exceptions.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to access localization sources and manage
        /// the application's language settings.
        /// </para>
        /// </remarks>
        public ILocalizationManager LocalizationManager { get; set; }

        /// <summary>
        /// Gets or sets the logger for application logging and debugging.
        /// </summary>
        /// <value>
        /// The logger instance that provides logging capabilities.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides access to the logging system, allowing services to record
        /// information, warnings, errors, and debug messages. The logger supports multiple
        /// log levels and can be configured to write to various outputs (files, databases, etc.).
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> Initialized with <see cref="NullLogger.Instance"/>
        /// to prevent null reference exceptions.
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> The setter is protected to allow derived classes
        /// to configure logging while preventing external modification.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to log application events, errors, and
        /// debugging information throughout the service lifecycle.
        /// </para>
        /// </remarks>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// Gets or sets the object mapper for object-to-object transformations.
        /// </summary>
        /// <value>
        /// The object mapper instance that provides mapping capabilities between different object types.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides access to the object mapping system, which facilitates
        /// transformations between different object types (e.g., entities to DTOs, view models
        /// to domain objects). The object mapper supports complex mapping scenarios and
        /// can be configured with custom mapping rules.
        /// </para>
        /// <para>
        /// <strong>Default Value:</strong> Initialized with <see cref="NullObjectMapper.Instance"/>
        /// to prevent null reference exceptions.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to map between different object representations,
        /// such as converting domain entities to data transfer objects or view models.
        /// </para>
        /// </remarks>
        public IObjectMapper ObjectMapper { get; set; }

        /// <summary>
        /// Gets the current active unit of work.
        /// </summary>
        /// <value>
        /// The currently active unit of work instance.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides direct access to the current active unit of work, allowing
        /// services to perform database operations within the current transaction scope.
        /// The current unit of work is managed by the <see cref="UnitOfWorkManager"/>.
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> This property is protected to ensure that only
        /// derived service classes can access the current unit of work.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this property to access the current database context,
        /// perform queries, and manage entities within the current transaction.
        /// </para>
        /// </remarks>
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

        /// <summary>
        /// Gets or sets the name of the localization source used by this service.
        /// </summary>
        /// <value>
        /// The name of the localization source that provides localized strings for this service.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property specifies which localization source should be used for string
        /// localization within this service. It must be set before using the localization
        /// methods (<see cref="L(string)"/>, <see cref="L(string, CultureInfo)"/>, etc.).
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> This property is protected to ensure that only
        /// derived service classes can configure the localization source.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Set this property in the constructor or initialization
        /// method of derived services to specify the appropriate localization source.
        /// </para>
        /// </remarks>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets the localization source instance for this service.
        /// </summary>
        /// <value>
        /// The localization source instance that provides localized strings.
        /// </value>
        /// <exception cref="KontecgException">
        /// Thrown when <see cref="LocalizationSourceName"/> has not been set before accessing this property.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This property provides access to the localization source that has been configured
        /// for this service. The localization source is lazily initialized and cached for
        /// performance optimization.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The property validates that <see cref="LocalizationSourceName"/>
        /// has been set before allowing access to the localization source.
        /// </para>
        /// <para>
        /// <strong>Caching:</strong> The localization source is cached after first access to
        /// improve performance for subsequent localization requests.
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> This property is protected to ensure that only
        /// derived service classes can access the localization source.
        /// </para>
        /// </remarks>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new KontecgException(
                        "Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }

        /// <summary>
        /// Gets a localized string for the given key name using the current language.
        /// </summary>
        /// <param name="name">
        /// The key name of the localized string to retrieve.
        /// </param>
        /// <returns>
        /// The localized string for the specified key in the current language.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves a localized string using the current application language.
        /// It uses the localization source configured for this service to look up the
        /// string value associated with the provided key.
        /// </para>
        /// <para>
        /// <strong>Requirements:</strong> The <see cref="LocalizationSourceName"/> property
        /// must be set before calling this method.
        /// </para>
        /// <para>
        /// <strong>Fallback:</strong> If the key is not found in the current language,
        /// the system may fall back to a default language or return the key itself.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this method to display user-facing messages,
        /// error messages, and other text that should be localized.
        /// </para>
        /// </remarks>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// Gets a localized string for the given key name with formatting arguments using the current language.
        /// </summary>
        /// <param name="name">
        /// The key name of the localized string to retrieve.
        /// </param>
        /// <param name="args">
        /// The formatting arguments to substitute into the localized string.
        /// </param>
        /// <returns>
        /// The localized string with the specified arguments substituted, in the current language.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves a localized string and formats it with the provided arguments
        /// using the current application language. The arguments are substituted into the
        /// string using standard .NET string formatting.
        /// </para>
        /// <para>
        /// <strong>Formatting:</strong> The localized string can contain placeholders (e.g., {0}, {1})
        /// that will be replaced with the corresponding arguments from the args array.
        /// </para>
        /// <para>
        /// <strong>Requirements:</strong> The <see cref="LocalizationSourceName"/> property
        /// must be set before calling this method.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this method for localized strings that contain
        /// dynamic content, such as user names, dates, or other variable information.
        /// </para>
        /// </remarks>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// Gets a localized string for the given key name using the specified culture.
        /// </summary>
        /// <param name="name">
        /// The key name of the localized string to retrieve.
        /// </param>
        /// <param name="culture">
        /// The culture information that specifies the language and formatting preferences.
        /// </param>
        /// <returns>
        /// The localized string for the specified key in the specified culture.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method retrieves a localized string using the specified culture, regardless
        /// of the current application language. This is useful when you need to display
        /// content in a specific language or format.
        /// </para>
        /// <para>
        /// <strong>Culture Support:</strong> The method supports any culture that has
        /// localized resources available in the configured localization source.
        /// </para>
        /// <para>
        /// <strong>Requirements:</strong> The <see cref="LocalizationSourceName"/> property
        /// must be set before calling this method.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this method when you need to display content in
        /// a specific language, such as for user preferences or internationalization features.
        /// </para>
        /// </remarks>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// Gets a localized string for the given key name with formatting arguments using the specified culture.
        /// </summary>
        /// <param name="name">
        /// The key name of the localized string to retrieve.
        /// </param>
        /// <param name="culture">
        /// The culture information that specifies the language and formatting preferences.
        /// </param>
        /// <param name="args">
        /// The formatting arguments to substitute into the localized string.
        /// </param>
        /// <returns>
        /// The localized string with the specified arguments substituted, in the specified culture.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method combines culture-specific localization with string formatting.
        /// It retrieves a localized string for the specified culture and formats it with
        /// the provided arguments using culture-appropriate formatting rules.
        /// </para>
        /// <para>
        /// <strong>Formatting:</strong> The localized string can contain placeholders (e.g., {0}, {1})
        /// that will be replaced with the corresponding arguments from the args array.
        /// The formatting follows the rules of the specified culture.
        /// </para>
        /// <para>
        /// <strong>Culture Support:</strong> The method supports any culture that has
        /// localized resources available in the configured localization source.
        /// </para>
        /// <para>
        /// <strong>Requirements:</strong> The <see cref="LocalizationSourceName"/> property
        /// must be set before calling this method.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> Use this method for culture-specific localized strings
        /// that contain dynamic content, ensuring proper formatting for the target culture.
        /// </para>
        /// </remarks>
        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }
    }
}
