using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Manages localization texts for host and companies.
    /// </summary>
    public interface IApplicationLanguageTextManager
    {
        /// <summary>
        ///     Gets a localized string value.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        /// <param name="sourceName">Source name</param>
        /// <param name="culture">Culture</param>
        /// <param name="key">Localization key</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        string GetStringOrNull(int? companyId, string sourceName, CultureInfo culture, string key,
            bool tryDefaults = true);

        /// <summary>
        ///     Gets list of localized strings value.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        /// <param name="sourceName">Source name</param>
        /// <param name="culture">Culture</param>
        /// <param name="keys">Localization keys</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        List<string> GetStringsOrNull(int? companyId, string sourceName, CultureInfo culture, List<string> keys,
            bool tryDefaults = true);

        /// <summary>
        ///     Updates a localized string value.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        /// <param name="sourceName">Source name</param>
        /// <param name="culture">Culture</param>
        /// <param name="key">Localization key</param>
        /// <param name="value">New localized value.</param>
        Task UpdateStringAsync(int? companyId, string sourceName, CultureInfo culture, string key, string value);
    }
}
