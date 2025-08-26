using System;
using System.Runtime.Serialization;

namespace Kontecg
{
    /// <summary>
    ///     Base exception type for those are thrown by Kontecg system for Kontecg specific exceptions.
    /// </summary>
    [Serializable]
    public class KontecgException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="KontecgException" /> object.
        /// </summary>
        public KontecgException()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgException" /> object.
        /// </summary>
        public KontecgException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
