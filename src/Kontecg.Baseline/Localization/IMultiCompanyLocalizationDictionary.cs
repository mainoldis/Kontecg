using System.Collections.Generic;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Extends <see cref="ILocalizationDictionary" /> to add company and database based localization.
    /// </summary>
    public interface IMultiCompanyLocalizationDictionary : ILocalizationDictionary
    {
        /// <summary>
        ///     Gets a <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="name">Localization key name.</param>
        LocalizedString GetOrNull(int? companyId, string name);

        /// <summary>
        ///     Gets a <see cref="LocalizedString" />.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        /// <param name="names">List of localization key names.</param>
        IReadOnlyList<LocalizedString> GetStringsOrNull(int? companyId, List<string> names);

        /// <summary>
        ///     Gets all <see cref="LocalizedString" />s.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host.</param>
        IReadOnlyList<LocalizedString> GetAllStrings(int? companyId);
    }
}
