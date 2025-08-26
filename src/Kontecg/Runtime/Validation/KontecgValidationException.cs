using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Kontecg.Logging;

namespace Kontecg.Runtime.Validation
{
    /// <summary>
    ///     This exception type is used to throws validation exceptions.
    /// </summary>
    [Serializable]
    public class KontecgValidationException : KontecgException, IHasLogSeverity
    {
        /// <summary>
        ///     Default log severity
        /// </summary>
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public KontecgValidationException()
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Constructor for serializing.
        /// </summary>
        public KontecgValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgValidationException(string message)
            : base(message)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="validationErrors">Validation errors</param>
        public KontecgValidationException(string message, IList<ValidationResult> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new List<ValidationResult>();
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Detailed list of validation errors for this exception.
        /// </summary>
        public IList<ValidationResult> ValidationErrors { get; set; }

        /// <summary>
        ///     Exception severity.
        ///     Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }
    }
}
