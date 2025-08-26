namespace Kontecg.ExceptionHandling
{
    public interface IExceptionHandlingConfiguration
    {
        /// <summary>
        ///     Used to enable/disable exception handling system.
        ///     Default: true. Set false to completely disable it.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Used to enable/disable propagate exception after intercepted.
        ///     Default: true. Set false to completely disable it.
        /// </summary>
        public bool PropagatedHandledExceptions { get; set; }

        /// <summary>
        ///     If this is set to true, all exception and details are sent directly to views on an error.
        ///     Default: false (Kontecg hides exception details from views except special exceptions.)
        /// </summary>
        bool SendDetailedExceptionsToSupport { get; set; }

        /// <summary>
        ///     If and exception is thrown, KontecgHandledExceptionData is handled
        /// </summary>
        public bool TriggerDomainEvents { get; set; }
    }
}
