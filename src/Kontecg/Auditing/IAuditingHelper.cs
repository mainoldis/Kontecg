using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kontecg.Auditing
{
    /// <summary>
    /// Defines the contract for auditing operations in the application.
    /// This interface provides methods for determining audit requirements, creating audit information,
    /// and persisting audit records for method invocations and data changes.
    /// </summary>
    /// <remarks>
    /// The IAuditingHelper interface is central to the application's auditing system, providing
    /// the ability to track method calls, parameter values, and execution results. It supports
    /// both synchronous and asynchronous audit operations and can be configured to selectively
    /// audit specific methods or types based on business requirements and performance considerations.
    /// </remarks>
    public interface IAuditingHelper
    {
        /// <summary>
        /// Determines whether a method invocation should be audited based on its metadata and configuration.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo object containing metadata about the method to be audited.</param>
        /// <param name="defaultValue">The default value to return if no specific audit configuration is found. Default is false.</param>
        /// <returns>
        /// True if the method should be audited; otherwise, false. The decision is based on
        /// method attributes, configuration settings, and auditing policies.
        /// </returns>
        /// <remarks>
        /// This method evaluates whether a specific method should be audited by examining
        /// its attributes, return type, parameter types, and any configured auditing rules.
        /// The decision can be influenced by performance considerations, security requirements,
        /// and business audit policies.
        /// </remarks>
        bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false);

        /// <summary>
        /// Creates audit information for a method invocation using an array of arguments.
        /// </summary>
        /// <param name="type">The type of the class containing the method being audited.</param>
        /// <param name="method">The MethodInfo object representing the method being audited.</param>
        /// <param name="arguments">An array of objects representing the method arguments.</param>
        /// <returns>
        /// An AuditInfo object containing detailed information about the method invocation,
        /// including parameter values, execution context, and metadata.
        /// </returns>
        /// <remarks>
        /// This method creates a comprehensive audit record for a method invocation, capturing
        /// the method signature, argument values, execution time, and contextual information.
        /// The audit information can be used for debugging, compliance, and security analysis.
        /// </remarks>
        AuditInfo CreateAuditInfo(Type type, MethodInfo method, object[] arguments);

        /// <summary>
        /// Creates audit information for a method invocation using a dictionary of named arguments.
        /// </summary>
        /// <param name="type">The type of the class containing the method being audited.</param>
        /// <param name="method">The MethodInfo object representing the method being audited.</param>
        /// <param name="arguments">A dictionary containing parameter names and their corresponding values.</param>
        /// <returns>
        /// An AuditInfo object containing detailed information about the method invocation,
        /// including named parameter values, execution context, and metadata.
        /// </returns>
        /// <remarks>
        /// This overload provides more detailed audit information by using named parameters,
        /// which can be useful for complex method signatures or when parameter names are
        /// important for audit analysis and debugging.
        /// </remarks>
        AuditInfo CreateAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments);

        /// <summary>
        /// Persists audit information to the audit storage system synchronously.
        /// </summary>
        /// <param name="auditInfo">The AuditInfo object containing the audit data to be persisted.</param>
        /// <remarks>
        /// This method saves the audit information to the configured audit storage (database,
        /// file system, or other storage mechanism). The operation is performed synchronously,
        /// which may block the calling thread until the audit data is successfully persisted.
        /// </remarks>
        void Save(AuditInfo auditInfo);

        /// <summary>
        /// Persists audit information to the audit storage system asynchronously.
        /// </summary>
        /// <param name="auditInfo">The AuditInfo object containing the audit data to be persisted.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task completes when
        /// the audit information has been successfully persisted to storage.
        /// </returns>
        /// <remarks>
        /// This method provides non-blocking audit persistence, allowing the application to
        /// continue execution while audit data is being saved. This is particularly useful
        /// for high-performance scenarios where audit operations should not impact the main
        /// application flow.
        /// </remarks>
        Task SaveAsync(AuditInfo auditInfo);
    }
}
