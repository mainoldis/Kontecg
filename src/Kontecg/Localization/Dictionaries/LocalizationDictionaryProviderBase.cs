using System.Collections.Generic;

namespace Kontecg.Localization.Dictionaries
{
    public abstract class LocalizationDictionaryProviderBase : ILocalizationDictionaryProvider
    {
        protected LocalizationDictionaryProviderBase()
        {
            Dictionaries = new Dictionary<string, ILocalizationDictionary>();
        }

        public string SourceName { get; private set; }

        public ILocalizationDictionary DefaultDictionary { get; protected set; }

        public IDictionary<string, ILocalizationDictionary> Dictionaries { get; }

        public void Initialize(string sourceName)
        {
            SourceName = sourceName;
            InitializeDictionaries();
        }

        public void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            if (!Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out ILocalizationDictionary existingDictionary))
            {
                Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            IReadOnlyList<LocalizedString> localizedStrings = dictionary.GetAllStrings();
            foreach (LocalizedString localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }

        protected virtual void InitializeDictionaries()
        {
        }

        protected virtual void InitializeDictionary<TDictionary>(TDictionary dictionary, bool isDefault = false)
            where TDictionary : ILocalizationDictionary
        {
            if (Dictionaries.ContainsKey(dictionary.CultureInfo.Name))
            {
                throw new KontecgInitializationException(SourceName +
                                                         " source contains more than one dictionary for the culture: " +
                                                         dictionary.CultureInfo.Name);
            }

            Dictionaries[dictionary.CultureInfo.Name] = dictionary;

            if (isDefault)
            {
                if (DefaultDictionary != null)
                {
                    throw new KontecgInitializationException(
                        "Only one default localization dictionary can be for source: " + SourceName);
                }

                DefaultDictionary = dictionary;
            }
        }
    }
}
