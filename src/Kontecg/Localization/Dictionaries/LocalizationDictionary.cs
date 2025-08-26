using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Kontecg.Localization.Dictionaries
{
    /// <summary>
    ///     Represents a simple implementation of <see cref="ILocalizationDictionary" /> interface.
    /// </summary>
    public class LocalizationDictionary : ILocalizationDictionary, IEnumerable<LocalizedString>
    {
        private readonly Dictionary<string, LocalizedString> _dictionary;

        /// <summary>
        ///     Creates a new <see cref="LocalizationDictionary" /> object.
        /// </summary>
        /// <param name="cultureInfo">Culture of the dictionary</param>
        public LocalizationDictionary(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
            _dictionary = new Dictionary<string, LocalizedString>();
        }

        /// <inheritdoc />
        public virtual IEnumerator<LocalizedString> GetEnumerator()
        {
            return GetAllStrings().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetAllStrings().GetEnumerator();
        }

        /// <inheritdoc />
        public CultureInfo CultureInfo { get; }

        /// <inheritdoc />
        public virtual string this[string name]
        {
            get
            {
                LocalizedString localizedString = GetOrNull(name);
                return localizedString?.Value;
            }
            set => _dictionary[name] = new LocalizedString(name, value, CultureInfo);
        }

        /// <inheritdoc />
        public virtual LocalizedString GetOrNull(string name)
        {
            return _dictionary.TryGetValue(name, out LocalizedString localizedString) ? localizedString : null;
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<LocalizedString> GetStringsOrNull(List<string> names)
        {
            return names.Select(name => _dictionary.Values.FirstOrDefault(x => x.Name == name) ??
                                        new LocalizedString(name, null, CultureInfo))
                .ToImmutableList();
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return _dictionary.Values.ToImmutableList();
        }

        protected bool Contains(string name)
        {
            return _dictionary.ContainsKey(name);
        }
    }
}
