using System;
using System.Runtime.Serialization;
using Kontecg.Logging;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     This exception is thrown on an unauthorized request.
    /// </summary>
    [Serializable]
    public class KontecgAuthorizationException : KontecgException, IHasLogSeverity
    {
        /// <summary>
        ///     Default log severity
        /// </summary>
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;

        /// <summary>
        ///     Creates a new <see cref="KontecgAuthorizationException" /> object.
        /// </summary>
        public KontecgAuthorizationException()
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgAuthorizationException" /> object.
        /// </summary>
        public KontecgAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgAuthorizationException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgAuthorizationException(string message)
            : base(message)
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgAuthorizationException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Severity of the exception.
        ///     Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }
    }
}
