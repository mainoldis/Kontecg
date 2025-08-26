using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Runtime.Session;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Implements <see cref="ILanguageProvider" /> to get languages from <see cref="IApplicationLanguageManager" />.
    /// </summary>
    public class ApplicationLanguageProvider : ILanguageProvider
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public ApplicationLanguageProvider(IApplicationLanguageManager applicationLanguageManager)
        {
            _applicationLanguageManager = applicationLanguageManager;

            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Reference to the session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        /// <summary>
        ///     Gets the languages for current company.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            List<LanguageInfo> languageInfos = _applicationLanguageManager.GetLanguages(KontecgSession.CompanyId)
                .OrderBy(l => l.DisplayName)
                .Select(l => l.ToLanguageInfo())
                .ToList();

            SetDefaultLanguage(languageInfos);

            return languageInfos;
        }

        /// <summary>
        ///     Gets the active languages for current company.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetActiveLanguages()
        {
            List<LanguageInfo> languageInfos = _applicationLanguageManager.GetActiveLanguages(KontecgSession.CompanyId)
                .OrderBy(l => l.DisplayName)
                .Select(l => l.ToLanguageInfo())
                .ToList();

            SetDefaultLanguage(languageInfos);

            return languageInfos;
        }

        /// <summary>
        ///     Gets the languages for current company.
        /// </summary>
        public async Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
        {
            List<LanguageInfo> languageInfos =
                (await _applicationLanguageManager.GetLanguagesAsync(KontecgSession.CompanyId))
                .OrderBy(l => l.DisplayName)
                .Select(l => l.ToLanguageInfo())
                .ToList();

            await SetDefaultLanguageAsync(languageInfos);

            return languageInfos;
        }

        private async Task SetDefaultLanguageAsync(List<LanguageInfo> languageInfos)
        {
            if (languageInfos.Count <= 0)
            {
                return;
            }

            ApplicationLanguage defaultLanguage =
                await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(KontecgSession.CompanyId);
            if (defaultLanguage == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            LanguageInfo languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
            if (languageInfo == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            languageInfo.IsDefault = true;
        }

        private void SetDefaultLanguage(List<LanguageInfo> languageInfos)
        {
            if (languageInfos.Count <= 0)
            {
                return;
            }

            ApplicationLanguage defaultLanguage =
                _applicationLanguageManager.GetDefaultLanguageOrNull(KontecgSession.CompanyId);
            if (defaultLanguage == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            LanguageInfo languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
            if (languageInfo == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            languageInfo.IsDefault = true;
        }
    }
}
