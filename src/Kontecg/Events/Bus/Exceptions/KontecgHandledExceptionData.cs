using System;

namespace Kontecg.Events.Bus.Exceptions
{
    /// <summary>
    ///     This type of events are used to notify for exceptions handled by Kontecg infrastructure.
    /// </summary>
    public class KontecgHandledExceptionData : ExceptionData
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public KontecgHandledExceptionData(Exception exception)
            : base(exception)
        {
        }
    }
}
