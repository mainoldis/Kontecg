using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Runtime.Validation;

namespace Kontecg.Logging
{
    /// <summary>
    ///     This class can be used to write logs from somewhere where it's a hard to get a reference to the
    ///     <see cref="ILogger" />.
    ///     Normally, use <see cref="ILogger" /> with property injection wherever it's possible.
    /// </summary>
    public static class LogHelper
    {
        static LogHelper()
        {
            Logger = IocManager.Instance.IsRegistered(typeof(ILoggerFactory))
                ? IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(LogHelper))
                : NullLogger.Instance;
        }

        /// <summary>
        ///     A reference to the logger.
        /// </summary>
        public static ILogger Logger { get; }

        public static void LogException(Exception ex)
        {
            LogException(Logger, ex);
        }

        public static void LogException(ILogger logger, Exception ex)
        {
            LogSeverity severity = (ex as IHasLogSeverity)?.Severity ?? LogSeverity.Error;

            logger.Log(severity, ex.Message, ex);

            LogValidationErrors(logger, ex);
        }

        private static void LogValidationErrors(ILogger logger, Exception exception)
        {
            //Try to find inner validation exception
            if (exception is AggregateException && exception.InnerException != null)
            {
                AggregateException aggException = exception as AggregateException;
                if (aggException.InnerException is KontecgValidationException)
                {
                    exception = aggException.InnerException;
                }
            }

            if (!(exception is KontecgValidationException))
            {
                return;
            }

            KontecgValidationException validationException = exception as KontecgValidationException;
            if (validationException.ValidationErrors.IsNullOrEmpty())
            {
                return;
            }

            logger.Log(validationException.Severity,
                "There are " + validationException.ValidationErrors.Count + " validation errors:");
            foreach (ValidationResult validationResult in validationException.ValidationErrors)
            {
                string memberNames = "";
                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                }

                logger.Log(validationException.Severity, validationResult.ErrorMessage + memberNames);
            }
        }
    }
}
