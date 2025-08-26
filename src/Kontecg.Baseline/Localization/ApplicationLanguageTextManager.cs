using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Localization.Sources;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Manages localization texts for host and companies.
    /// </summary>
    public class ApplicationLanguageTextManager : IApplicationLanguageTextManager, ITransientDependency
    {
        private readonly IRepository<ApplicationLanguageText, long> _applicationTextRepository;
        private readonly ILocalizationManager _localizationManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationLanguageTextManager" /> class.
        /// </summary>
        public ApplicationLanguageTextManager(
            ILocalizationManager localizationManager,
            IRepository<ApplicationLanguageText, long> applicationTextRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _localizationManager = localizationManager;
            _applicationTextRepository = applicationTextRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        ///     Gets a localized string value.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        /// <param name="sourceName">Source name</param>
        /// <param name="culture">Culture</param>
        /// <param name="key">Localization key</param>
        /// <param name="tryDefaults">True: fallbacks to default languages if can not find in given culture</param>
        public string GetStringOrNull(int? companyId, string sourceName, CultureInfo culture, string key,
            bool tryDefaults = true)
        {
            ILocalizationSource source = _localizationManager.GetSource(sourceName);

            if (!(source is IMultiCompanyLocalizationSource))
            {
                return source.GetStringOrNull(key, culture, tryDefaults);
            }

            return source
                .As<IMultiCompanyLocalizationSource>()
                .GetStringOrNull(companyId, key, culture, tryDefaults);
        }

        public List<string> GetStringsOrNull(int? companyId, string sourceName, CultureInfo culture, List<string> keys,
            bool tryDefaults = true)
        {
            ILocalizationSource source = _localizationManager.GetSource(sourceName);

            if (!(source is IMultiCompanyLocalizationSource))
            {
                return source.GetStringsOrNull(keys, culture, tryDefaults);
            }

            return source
                .As<IMultiCompanyLocalizationSource>()
                .GetStringsOrNull(companyId, keys, culture, tryDefaults);
        }

        /// <summary>
        ///     Updates a localized string value.
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        /// <param name="sourceName">Source name</param>
        /// <param name="culture">Culture</param>
        /// <param name="key">Localization key</param>
        /// <param name="value">New localized value.</param>
        public virtual async Task UpdateStringAsync(int? companyId, string sourceName, CultureInfo culture, string key,
            string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    ApplicationLanguageText existingEntity = (await _applicationTextRepository.GetAllListAsync(t =>
                            t.Source == sourceName &&
                            t.LanguageName == culture.Name &&
                            t.Key == key))
                        .FirstOrDefault(t => t.Key == key);

                    if (existingEntity != null)
                    {
                        if (existingEntity.Value != value)
                        {
                            existingEntity.Value = value;
                            await _unitOfWorkManager.Current.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        await _applicationTextRepository.InsertAsync(
                            new ApplicationLanguageText
                            {
                                CompanyId = companyId,
                                Source = sourceName,
                                LanguageName = culture.Name,
                                Key = key,
                                Value = value
                            });
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }
    }
}
