using Kontecg.Localization.Dictionaries;

namespace Kontecg.Localization.Sources
{
    /// <summary>
    ///     Used to store a localization source extension information.
    /// </summary>
    public class LocalizationSourceExtensionInfo
    {
        /// <summary>
        ///     Creates a new <see cref="LocalizationSourceExtensionInfo" /> object.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="dictionaryProvider">Extension dictionaries</param>
        public LocalizationSourceExtensionInfo(string sourceName, ILocalizationDictionaryProvider dictionaryProvider)
        {
            SourceName = sourceName;
            DictionaryProvider = dictionaryProvider;
        }

        /// <summary>
        ///     Source name.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        ///     Extension dictionaries.
        /// </summary>
        public ILocalizationDictionaryProvider DictionaryProvider { get; }
    }
}
