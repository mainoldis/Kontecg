using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontecg.Dependency;

namespace Kontecg.Localization
{
    public class LanguageManager : ILanguageManager, ITransientDependency
    {
        private readonly ILanguageProvider _languageProvider;

        public LanguageManager(ILanguageProvider languageProvider)
        {
            _languageProvider = languageProvider;
        }

        public LanguageInfo CurrentLanguage => GetCurrentLanguage();

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return _languageProvider.GetLanguages();
        }

        public IReadOnlyList<LanguageInfo> GetActiveLanguages()
        {
            return _languageProvider.GetActiveLanguages();
        }

        private LanguageInfo GetCurrentLanguage()
        {
            IReadOnlyList<LanguageInfo> languages = _languageProvider.GetLanguages();
            if (languages.Count <= 0)
            {
                throw new KontecgException("No language defined in this application.");
            }

            string currentCultureName = CultureInfo.CurrentUICulture.Name;

            //Try to find exact match
            LanguageInfo currentLanguage = languages.FirstOrDefault(l => l.Name == currentCultureName);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find best match
            currentLanguage = languages.FirstOrDefault(l => currentCultureName.StartsWith(l.Name));
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find default language
            currentLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Get first one
            return languages[0];
        }
    }
}
