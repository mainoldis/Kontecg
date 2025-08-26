using System.Collections.Generic;
using Kontecg.Localization;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Used for localization configurations.
    /// </summary>
    internal class LocalizationConfiguration : ILocalizationConfiguration
    {
        public LocalizationConfiguration()
        {
            Languages = new List<LanguageInfo>();
            Sources = new LocalizationSourceList();

            IsEnabled = true;
            ReturnGivenTextIfNotFound = true;
            WrapGivenTextIfNotFound = true;
            HumanizeTextIfNotFound = true;
            LogWarnMessageIfNotFound = true;
        }

        /// <inheritdoc />
        public IList<LanguageInfo> Languages { get; }

        /// <inheritdoc />
        public ILocalizationSourceList Sources { get; }

        /// <inheritdoc />
        public bool IsEnabled { get; set; }

        /// <inheritdoc />
        public bool ReturnGivenTextIfNotFound { get; set; }

        /// <inheritdoc />
        public bool WrapGivenTextIfNotFound { get; set; }

        /// <inheritdoc />
        public bool HumanizeTextIfNotFound { get; set; }

        public bool LogWarnMessageIfNotFound { get; set; }
    }
}
