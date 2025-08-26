using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Manages host and company languages.
    /// </summary>
    public interface IApplicationLanguageManager
    {
        /// <summary>
        ///     Gets list of all languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        Task<IReadOnlyList<ApplicationLanguage>> GetLanguagesAsync(int? companyId);

        /// <summary>
        ///     Gets list of all active languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        Task<IReadOnlyList<ApplicationLanguage>> GetActiveLanguagesAsync(int? companyId);

        /// <summary>
        ///     Gets list of all languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        IReadOnlyList<ApplicationLanguage> GetLanguages(int? companyId);

        /// <summary>
        ///     Gets list of all active languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        IReadOnlyList<ApplicationLanguage> GetActiveLanguages(int? companyId);

        /// <summary>
        ///     Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        Task AddAsync(ApplicationLanguage language);

        /// <summary>
        ///     Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        void Add(ApplicationLanguage language);

        /// <summary>
        ///     Deletes a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        Task RemoveAsync(int? companyId, string languageName);

        /// <summary>
        ///     Deletes a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        void Remove(int? companyId, string languageName);

        /// <summary>
        ///     Updates a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="language">The language to be updated</param>
        Task UpdateAsync(int? companyId, ApplicationLanguage language);

        /// <summary>
        ///     Updates a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="language">The language to be updated</param>
        void Update(int? companyId, ApplicationLanguage language);

        /// <summary>
        ///     Gets the default language or null for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        Task<ApplicationLanguage> GetDefaultLanguageOrNullAsync(int? companyId);

        /// <summary>
        ///     Gets the default language or null for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        ApplicationLanguage GetDefaultLanguageOrNull(int? companyId);

        /// <summary>
        ///     Sets the default language for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        Task SetDefaultLanguageAsync(int? companyId, string languageName);

        /// <summary>
        ///     Sets the default language for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        void SetDefaultLanguage(int? companyId, string languageName);
    }
}
