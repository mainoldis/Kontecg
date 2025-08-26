using System.Collections.Generic;
using System.Globalization;
using Kontecg.Extensions;
using Kontecg.FluentValidation.Configuration;
using Kontecg.Localization;
using Kontecg.Localization.Sources;
using LanguageManager = FluentValidation.Resources.LanguageManager;

namespace Kontecg.FluentValidation
{
    public class KontecgFluentValidationLanguageManager : LanguageManager
    {
        public KontecgFluentValidationLanguageManager(
            ILocalizationManager localizationManager,
            ILanguageManager languageManager,
            IKontecgFluentValidationConfiguration configuration)
        {
            if (!configuration.LocalizationSourceName.IsNullOrEmpty())
            {
                ILocalizationSource source = localizationManager.GetSource(configuration.LocalizationSourceName);
                IReadOnlyList<LanguageInfo> languages = languageManager.GetActiveLanguages();

                AddAllTranslations(source, languages);
            }
        }

        private void AddAllTranslations(ILocalizationSource source, IReadOnlyList<LanguageInfo> languages)
        {
            foreach (LanguageInfo language in languages)
            {
                CultureInfo culture = new CultureInfo(language.Name);
                IReadOnlyList<LocalizedString> translations = source.GetAllStrings(culture, false);
                AddTranslations(language.Name, translations);
            }
        }

        private void AddTranslations(string language, IReadOnlyList<LocalizedString> translations)
        {
            foreach (LocalizedString translation in translations)
            {
                AddTranslation(language, translation.Name, translation.Value);
            }
        }
    }
}
