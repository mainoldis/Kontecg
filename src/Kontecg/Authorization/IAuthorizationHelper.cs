using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kontecg.Authorization
{
    /// <summary>
    /// Defines the contract for authorization operations in the application.
    /// This interface provides methods for checking user permissions and authorizing
    /// access to methods and resources based on configured authorization attributes.
    /// </summary>
    /// <remarks>
    /// The IAuthorizationHelper interface is central to the application's security system,
    /// providing the ability to enforce access control policies at the method level.
    /// It supports both synchronous and asynchronous authorization checks and can work
    /// with various types of authorization attributes to implement flexible security policies.
    /// </remarks>
    public interface IAuthorizationHelper
    {
        /// <summary>
        /// Asynchronously authorizes access based on a collection of authorization attributes.
        /// </summary>
        /// <param name="authorizeAttributes">A collection of IKontecgAuthorizeAttribute objects
        /// that define the authorization requirements.</param>
        /// <returns>
        /// A task that represents the asynchronous authorization operation. The task completes
        /// when the authorization check has been performed.
        /// </returns>
        /// <exception cref="KontecgAuthorizationException">Thrown when the user is not authorized
        /// to access the requested resource or perform the requested operation.</exception>
        /// <remarks>
        /// This method evaluates all provided authorization attributes against the current user's
        /// permissions and roles. If any attribute indicates that authorization should be denied,
        /// the method will throw a KontecgAuthorizationException. The method is asynchronous
        /// to support authorization checks that may involve external services or databases.
        /// </remarks>
        Task AuthorizeAsync(IEnumerable<IKontecgAuthorizeAttribute> authorizeAttributes);

        /// <summary>
        /// Synchronously authorizes access based on a collection of authorization attributes.
        /// </summary>
        /// <param name="authorizeAttributes">A collection of IKontecgAuthorizeAttribute objects
        /// that define the authorization requirements.</param>
        /// <exception cref="KontecgAuthorizationException">Thrown when the user is not authorized
        /// to access the requested resource or perform the requested operation.</exception>
        /// <remarks>
        /// This method performs a synchronous authorization check against all provided
        /// authorization attributes. It evaluates the current user's permissions and roles
        /// and throws a KontecgAuthorizationException if access should be denied. This method
        /// is useful for scenarios where immediate authorization feedback is required.
        /// </remarks>
        void Authorize(IEnumerable<IKontecgAuthorizeAttribute> authorizeAttributes);

        /// <summary>
        /// Asynchronously authorizes access based on method and type metadata.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo object representing the method being accessed.</param>
        /// <param name="type">The Type object representing the class containing the method.</param>
        /// <returns>
        /// A task that represents the asynchronous authorization operation. The task completes
        /// when the authorization check has been performed.
        /// </returns>
        /// <exception cref="KontecgAuthorizationException">Thrown when the user is not authorized
        /// to access the requested method or type.</exception>
        /// <remarks>
        /// This method extracts authorization attributes from the method and type metadata
        /// and performs an authorization check. It automatically discovers IKontecgAuthorizeAttribute
        /// instances applied to the method, its parameters, and the containing type. The method
        /// is asynchronous to support authorization checks that may involve external services.
        /// </remarks>
        Task AuthorizeAsync(MethodInfo methodInfo, Type type);

        /// <summary>
        /// Synchronously authorizes access based on method and type metadata.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo object representing the method being accessed.</param>
        /// <param name="type">The Type object representing the class containing the method.</param>
        /// <exception cref="KontecgAuthorizationException">Thrown when the user is not authorized
        /// to access the requested method or type.</exception>
        /// <remarks>
        /// This method performs a synchronous authorization check by examining the method
        /// and type metadata for authorization attributes. It automatically discovers and
        /// evaluates all IKontecgAuthorizeAttribute instances applied to the method, its
        /// parameters, and the containing type. This method is useful for scenarios where
        /// immediate authorization feedback is required and the authorization logic is
        /// relatively simple and fast.
        /// </remarks>
        void Authorize(MethodInfo methodInfo, Type type);
    }
}
