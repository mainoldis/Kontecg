using System;
using System.Runtime.Serialization;

namespace Kontecg.Domain.Uow
{
    [Serializable]
    public class KontecgDbConcurrencyException : KontecgException
    {
        /// <summary>
        /// Creates a new <see cref="KontecgDbConcurrencyException"/> object.
        /// </summary>
        public KontecgDbConcurrencyException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="KontecgException"/> object.
        /// </summary>
        public KontecgDbConcurrencyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="KontecgDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgDbConcurrencyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="KontecgDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
