using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Kontecg.BackgroundJobs
{
    [Serializable]
    public class BackgroundJobException : KontecgException
    {
        /// <summary>
        ///     Creates a new <see cref="BackgroundJobException" /> object.
        /// </summary>
        public BackgroundJobException()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="BackgroundJobException" /> object.
        /// </summary>
        public BackgroundJobException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="BackgroundJobException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public BackgroundJobException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [CanBeNull] public BackgroundJobInfo BackgroundJob { get; set; }

        [CanBeNull] public object JobObject { get; set; }
    }
}
