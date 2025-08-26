using System.Collections.Generic;
using System.Globalization;
using Kontecg.Localization.Sources;

namespace Kontecg.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        private readonly IReadOnlyList<LanguageInfo> _emptyLanguageArray = new LanguageInfo[0];

        private readonly IReadOnlyList<ILocalizationSource> _emptyLocalizationSourceArray = new ILocalizationSource[0];

        private NullLocalizationManager()
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get; } = new();

        public LanguageInfo CurrentLanguage =>
            new(CultureInfo.CurrentUICulture.Name, CultureInfo.CurrentUICulture.DisplayName);

        public ILocalizationSource GetSource(string name)
        {
            return NullLocalizationSource.Instance;
        }

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _emptyLocalizationSourceArray;
        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return _emptyLanguageArray;
        }
    }
}
