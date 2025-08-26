using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the contract for determining whether configuration settings should be visible
    /// to client applications or user interfaces.
    /// </summary>
    /// <remarks>
    /// The ISettingClientVisibilityProvider interface is used to control the visibility
    /// of configuration settings in client-side applications, user interfaces, or API
    /// responses. This allows for fine-grained control over which settings are exposed
    /// to end users, helping to maintain security and reduce information disclosure.
    /// Implementations can use various criteria such as user roles, application context,
    /// or security policies to determine setting visibility. This is particularly useful
    /// in multi-tenant applications or scenarios where different users should have access
    /// to different configuration options.
    /// </remarks>
    public interface ISettingClientVisibilityProvider
    {
        /// <summary>
        /// Asynchronously determines whether configuration settings should be visible
        /// to the client based on the current scope and context.
        /// </summary>
        /// <param name="scope">The scoped IoC resolver that provides access to the current
        /// request context, user information, and other dependencies needed to make
        /// the visibility determination.</param>
        /// <returns>
        /// A task that represents the asynchronous visibility check operation. The task
        /// result is true if the settings should be visible to the client; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method is called by the configuration system to determine whether
        /// settings should be included in client responses or user interfaces. The scope
        /// parameter provides access to the current request context, allowing the provider
        /// to make decisions based on user identity, roles, permissions, or other contextual
        /// information. The method is asynchronous to support visibility checks that may
        /// involve database queries, external service calls, or other I/O operations.
        /// </remarks>
        Task<bool> CheckVisibleAsync(IScopedIocResolver scope);
    }
}
