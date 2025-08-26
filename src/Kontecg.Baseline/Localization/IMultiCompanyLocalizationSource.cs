using System.Collections.Generic;
using System.Globalization;
using Kontecg.Localization.Sources;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Extends <see cref="ILocalizationSource" /> to add company and database based localization.
    /// </summary>
    public interface IMultiCompanyLocalizationSource : ILocalizationSource
    {
        /// <summary>
        ///     Gets a <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="name">Localization key name.</param>
        /// <param name="culture">Culture</param>
        string GetString(int? companyId, string name, CultureInfo culture);

        /// <summary>
        ///     Gets a <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="name">Localization key name.</param>
        /// <param name="culture">Culture</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        string GetStringOrNull(int? companyId, string name, CultureInfo culture, bool tryDefaults = true);

        /// <summary>
        ///     Gets list of <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="names">Localization key name.</param>
        /// <param name="culture">Culture</param>
        List<string> GetStrings(int? companyId, List<string> names, CultureInfo culture);

        /// <summary>
        ///     Gets list of <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="names">Localization key name.</param>
        /// <param name="culture">Culture</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        List<string> GetStringsOrNull(int? companyId, List<string> names, CultureInfo culture, bool tryDefaults = true);
    }
}
