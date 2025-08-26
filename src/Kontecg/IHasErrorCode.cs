namespace Kontecg
{
    /// <summary>
    /// Defines a contract for objects that can carry error code information.
    /// This interface is commonly used in error handling scenarios where numeric error codes
    /// need to be associated with error objects or exceptions.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are typically used in conjunction with error handling
    /// mechanisms to provide standardized error identification across the application.
    /// The error code can be used for logging, debugging, or user-facing error messages.
    /// </remarks>
    public interface IHasErrorCode
    {
        /// <summary>
        /// Gets or sets the numeric error code associated with this object.
        /// </summary>
        /// <value>
        /// An integer representing the error code. Positive values typically indicate
        /// application-specific errors, while negative values may indicate system errors.
        /// </value>
        /// <remarks>
        /// The error code should be unique within the context of the application or module
        /// to facilitate proper error identification and handling.
        /// </remarks>
        int Code { get; set; }
    }
}
