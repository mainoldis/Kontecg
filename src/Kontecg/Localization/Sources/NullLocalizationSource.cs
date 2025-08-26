using System.Collections.Generic;
using System.Globalization;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;

namespace Kontecg.Localization.Sources
{
    /// <summary>
    ///     Null object pattern for <see cref="ILocalizationSource" />.
    /// </summary>
    internal class NullLocalizationSource : ILocalizationSource
    {
        private readonly IReadOnlyList<LocalizedString> _emptyStringArray = new LocalizedString[0];

        private NullLocalizationSource()
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullLocalizationSource Instance { get; } = new();

        public string Name => null;

        public void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
        }

        public string GetString(string name)
        {
            return name;
        }

        public string GetString(string name, CultureInfo culture)
        {
            return name;
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            return null;
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            return null;
        }

        public List<string> GetStrings(List<string> names)
        {
            return names;
        }

        public List<string> GetStrings(List<string> names, CultureInfo culture)
        {
            return names;
        }

        public List<string> GetStringsOrNull(List<string> names, bool tryDefaults = true)
        {
            return null;
        }

        public List<string> GetStringsOrNull(List<string> names, CultureInfo culture, bool tryDefaults = true)
        {
            return null;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            return _emptyStringArray;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            return _emptyStringArray;
        }
    }
}
