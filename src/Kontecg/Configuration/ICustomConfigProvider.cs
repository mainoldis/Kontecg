using System.Collections.Generic;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the contract for custom configuration providers that can supply additional
    /// configuration values beyond the standard application settings.
    /// </summary>
    /// <remarks>
    /// The ICustomConfigProvider interface allows for dynamic configuration loading from
    /// various sources such as external services, databases, or custom logic. Implementations
    /// of this interface can provide configuration values that are not available through
    /// the standard configuration system, enabling flexible and extensible configuration
    /// management. This is particularly useful for scenarios where configuration values
    /// need to be computed, retrieved from external systems, or dynamically generated
    /// based on runtime conditions.
    /// </remarks>
    public interface ICustomConfigProvider
    {
        /// <summary>
        /// Retrieves custom configuration values based on the provided context.
        /// </summary>
        /// <param name="customConfigProviderContext">The context object containing information
        /// about the configuration request, including any parameters or conditions that
        /// may affect the configuration values to be returned.</param>
        /// <returns>
        /// A dictionary containing configuration key-value pairs. The keys represent
        /// configuration setting names, and the values represent the corresponding
        /// configuration values.
        /// </returns>
        /// <remarks>
        /// This method is called by the configuration system when custom configuration
        /// values are needed. The context parameter provides information about the current
        /// request, such as user context, application state, or other factors that may
        /// influence the configuration values. The returned dictionary should contain
        /// all relevant configuration values that this provider is responsible for.
        /// Configuration values returned by this method will be merged with the standard
        /// configuration system and made available throughout the application.
        /// </remarks>
        Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext);
    }
}
