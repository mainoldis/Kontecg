using System;
using System.Runtime.Serialization;

namespace Kontecg
{
    /// <summary>
    ///     This exception is thrown if a problem on Kontecg initialization progress.
    /// </summary>
    [Serializable]
    public class KontecgInitializationException : KontecgException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public KontecgInitializationException()
        {
        }

        /// <summary>
        ///     Constructor for serializing.
        /// </summary>
        public KontecgInitializationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgInitializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
