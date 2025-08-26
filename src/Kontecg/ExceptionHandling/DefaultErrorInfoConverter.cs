using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Kontecg.Localization;
using Kontecg.Runtime.Validation;
using Kontecg.UI;

namespace Kontecg.ExceptionHandling
{
    internal class DefaultErrorInfoConverter : IExceptionToErrorInfoConverter
    {
        private readonly IExceptionHandlingConfiguration _configuration;
        private readonly ILocalizationManager _localizationManager;

        public DefaultErrorInfoConverter(
            IExceptionHandlingConfiguration configuration,
            ILocalizationManager localizationManager)
        {
            _configuration = configuration;
            _localizationManager = localizationManager;
        }

        private bool SendAllExceptionsToClients => _configuration.SendDetailedExceptionsToSupport;

        public IExceptionToErrorInfoConverter Next { set; private get; }

        public ErrorInfo Convert(Exception exception)
        {
            ErrorInfo errorInfo = CreateErrorInfoWithoutCode(exception);

            if (exception is IHasErrorCode)
            {
                errorInfo.Code = (exception as IHasErrorCode).Code;
            }

            return errorInfo;
        }

        private ErrorInfo CreateErrorInfoWithoutCode(Exception exception)
        {
            if (SendAllExceptionsToClients)
            {
                return CreateDetailedErrorInfoFromException(exception);
            }

            if (exception is AggregateException && exception.InnerException != null)
            {
                AggregateException aggException = exception as AggregateException;
                if (aggException.InnerException is UserFriendlyException ||
                    aggException.InnerException is KontecgValidationException)
                {
                    exception = aggException.InnerException;
                }
            }

            if (exception is UserFriendlyException)
            {
                UserFriendlyException userFriendlyException = exception as UserFriendlyException;
                return new ErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            }

            if (exception is KontecgValidationException)
            {
                return new ErrorInfo(L("ValidationError"))
                {
                    ValidationErrors = GetValidationErrorInfos(exception as KontecgValidationException),
                    Details = GetValidationErrorNarrative(exception as KontecgValidationException)
                };
            }

            if (exception is EntityNotFoundException)
            {
                EntityNotFoundException entityNotFoundException = exception as EntityNotFoundException;

                if (entityNotFoundException.EntityType != null)
                {
                    return new ErrorInfo(
                        string.Format(
                            L("EntityNotFound"),
                            entityNotFoundException.EntityType.Name,
                            entityNotFoundException.Id
                        )
                    );
                }

                return new ErrorInfo(
                    entityNotFoundException.Message
                );
            }

            if (exception is KontecgAuthorizationException)
            {
                KontecgAuthorizationException authorizationException = exception as KontecgAuthorizationException;
                return new ErrorInfo(authorizationException.Message);
            }

            return new ErrorInfo(L("InternalServerError"));
        }

        private ErrorInfo CreateDetailedErrorInfoFromException(Exception exception)
        {
            StringBuilder detailBuilder = new StringBuilder();

            AddExceptionToDetails(exception, detailBuilder);

            ErrorInfo errorInfo = new ErrorInfo(exception.Message, detailBuilder.ToString());

            if (exception is KontecgValidationException)
            {
                errorInfo.ValidationErrors = GetValidationErrorInfos(exception as KontecgValidationException);
            }

            return errorInfo;
        }

        private void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
        {
            //Exception Message
            detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

            //Additional info for UserFriendlyException
            if (exception is UserFriendlyException)
            {
                UserFriendlyException userFriendlyException = exception as UserFriendlyException;
                if (!string.IsNullOrEmpty(userFriendlyException.Details))
                {
                    detailBuilder.AppendLine(userFriendlyException.Details);
                }
            }

            //Additional info for KontecgValidationException
            if (exception is KontecgValidationException)
            {
                KontecgValidationException validationException = exception as KontecgValidationException;
                if (validationException.ValidationErrors.Count > 0)
                {
                    detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
                }
            }

            //Exception StackTrace
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
            }

            //Inner exception
            if (exception.InnerException != null)
            {
                AddExceptionToDetails(exception.InnerException, detailBuilder);
            }

            //Inner exceptions for AggregateException
            if (exception is AggregateException)
            {
                AggregateException aggException = exception as AggregateException;
                if (aggException.InnerExceptions.IsNullOrEmpty())
                {
                    return;
                }

                foreach (Exception innerException in aggException.InnerExceptions)
                {
                    AddExceptionToDetails(innerException, detailBuilder);
                }
            }
        }

        private ValidationErrorInfo[] GetValidationErrorInfos(KontecgValidationException validationException)
        {
            List<ValidationErrorInfo> validationErrorInfos = new List<ValidationErrorInfo>();

            foreach (ValidationResult validationResult in validationException.ValidationErrors)
            {
                ValidationErrorInfo validationError = new ValidationErrorInfo(validationResult.ErrorMessage);

                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
                }

                validationErrorInfos.Add(validationError);
            }

            return validationErrorInfos.ToArray();
        }

        private string GetValidationErrorNarrative(KontecgValidationException validationException)
        {
            StringBuilder detailBuilder = new StringBuilder();
            detailBuilder.AppendLine(L("ValidationNarrativeTitle"));

            foreach (ValidationResult validationResult in validationException.ValidationErrors)
            {
                detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
                detailBuilder.AppendLine();
            }

            return detailBuilder.ToString();
        }

        private string L(string name)
        {
            try
            {
                return _localizationManager.GetString(KontecgConsts.LocalizationSourceName, name);
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}
