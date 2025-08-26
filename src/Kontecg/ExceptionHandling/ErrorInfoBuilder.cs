using System;
using Kontecg.Dependency;
using Kontecg.Localization;

namespace Kontecg.ExceptionHandling
{
    public class ErrorInfoBuilder : IErrorInfoBuilder, ITransientDependency
    {
        public ErrorInfoBuilder(
            IExceptionHandlingConfiguration configuration,
            ILocalizationManager localizationManager)
        {
            Converter = new DefaultErrorInfoConverter(configuration, localizationManager);
        }

        private IExceptionToErrorInfoConverter Converter { get; set; }

        /// <inheritdoc />
        public ErrorInfo BuildForException(Exception exception)
        {
            return Converter.Convert(exception);
        }

        /// <summary>
        ///     Adds an exception converter that is used by <see cref="BuildForException" /> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = Converter;
            Converter = converter;
        }
    }
}
