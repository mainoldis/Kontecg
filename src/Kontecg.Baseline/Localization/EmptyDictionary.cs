using System.Collections.Generic;
using System.Globalization;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Localization
{
    internal class EmptyDictionary : ILocalizationDictionary
    {
        public EmptyDictionary(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
        }

        public CultureInfo CultureInfo { get; }

        public LocalizedString GetOrNull(string name)
        {
            return null;
        }

        public IReadOnlyList<LocalizedString> GetStringsOrNull(List<string> names)
        {
            return new LocalizedString[0];
        }

        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return new LocalizedString[0];
        }

        public string this[string name]
        {
            get => null;
            set { }
        }
    }
}
