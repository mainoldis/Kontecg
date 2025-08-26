using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Extends <see cref="ILocalizationDictionaryProvider" /> to add company and database based localization.
    /// </summary>
    public class MultiCompanyLocalizationDictionaryProvider : ILocalizationDictionaryProvider
    {
        private readonly ConcurrentDictionary<string, ILocalizationDictionary> _dictionaries;

        private readonly ILocalizationDictionaryProvider _internalProvider;

        private readonly IIocManager _iocManager;
        private ILanguageManager _languageManager;

        private string _sourceName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiCompanyLocalizationDictionaryProvider" /> class.
        /// </summary>
        public MultiCompanyLocalizationDictionaryProvider(ILocalizationDictionaryProvider internalProvider,
            IIocManager iocManager)
        {
            _internalProvider = internalProvider;
            _iocManager = iocManager;
            _dictionaries = new ConcurrentDictionary<string, ILocalizationDictionary>();
        }

        public ILocalizationDictionary DefaultDictionary => GetDefaultDictionary();

        public IDictionary<string, ILocalizationDictionary> Dictionaries => GetDictionaries();

        public void Initialize(string sourceName)
        {
            _sourceName = sourceName;
            _languageManager = _iocManager.Resolve<ILanguageManager>();
            _internalProvider.Initialize(_sourceName);
        }

        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            if (!_internalProvider.Dictionaries.TryGetValue(dictionary.CultureInfo.Name,
                    out ILocalizationDictionary existingDictionary))
            {
                _internalProvider.Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            IReadOnlyList<LocalizedString> localizedStrings = dictionary.GetAllStrings();
            foreach (LocalizedString localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }

        protected virtual IDictionary<string, ILocalizationDictionary> GetDictionaries()
        {
            IReadOnlyList<LanguageInfo> languages = _languageManager.GetActiveLanguages();

            foreach (LanguageInfo language in languages)
            {
                _dictionaries.GetOrAdd(language.Name, s => CreateLocalizationDictionary(language));
            }

            return _dictionaries;
        }

        protected virtual ILocalizationDictionary GetDefaultDictionary()
        {
            IReadOnlyList<LanguageInfo> languages = _languageManager.GetLanguages();
            if (!languages.Any())
            {
                throw new KontecgException("No language defined!");
            }

            LanguageInfo defaultLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (defaultLanguage == null)
            {
                throw new KontecgException("Default language is not defined!");
            }

            return _dictionaries.GetOrAdd(defaultLanguage.Name, s => CreateLocalizationDictionary(defaultLanguage));
        }

        protected virtual IMultiCompanyLocalizationDictionary CreateLocalizationDictionary(LanguageInfo language)
        {
            ILocalizationDictionary internalDictionary =
                _internalProvider.Dictionaries.GetOrDefault(language.Name) ??
                new EmptyDictionary(CultureInfo.GetCultureInfo(language.Name));

            IMultiCompanyLocalizationDictionary dictionary = _iocManager.Resolve<IMultiCompanyLocalizationDictionary>(
                new
                {
                    sourceName = _sourceName, internalDictionary
                });

            return dictionary;
        }
    }
}
