using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Configuration.Startup;
using Kontecg.Extensions;
using Kontecg.Logging;

namespace Kontecg.Localization
{
    public static class LocalizationSourceHelper
    {
        public static string ReturnGivenNameOrThrowException(
            ILocalizationConfiguration configuration,
            string sourceName,
            string name,
            CultureInfo culture,
            ILogger logger = null)
        {
            string exceptionMessage = $"Can not find '{name}' in localization source '{sourceName}'!";

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new KontecgException(exceptionMessage);
            }

            if (configuration.LogWarnMessageIfNotFound)
            {
                (logger ?? LogHelper.Logger).Warn(exceptionMessage);
            }

            string notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase(culture)
                : name;

            return configuration.WrapGivenTextIfNotFound
                ? $"[{notFoundText}]"
                : notFoundText;
        }

        public static List<string> ReturnGivenNamesOrThrowException(
            ILocalizationConfiguration configuration,
            string sourceName,
            List<string> names,
            CultureInfo culture,
            ILogger logger = null)
        {
            string exceptionMessage =
                $"Can not find '{string.Join(",", names)}' in localization source '{sourceName}' with culture '{culture.Name}'!";

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new KontecgException(exceptionMessage);
            }

            if (configuration.LogWarnMessageIfNotFound)
            {
                (logger ?? LogHelper.Logger).Warn(exceptionMessage);
            }

            List<string> notFoundText = configuration.HumanizeTextIfNotFound
                ? names.Select(name => name.ToSentenceCase(culture)).ToList()
                : names;

            return configuration.WrapGivenTextIfNotFound
                ? notFoundText.Select(text => $"[{text}]").ToList()
                : notFoundText;
        }
    }
}
