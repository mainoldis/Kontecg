using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Runtime.Caching;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Manages host and company languages.
    /// </summary>
    public class ApplicationLanguageManager :
        IApplicationLanguageManager,
        IEventHandler<EntityChangedEventData<ApplicationLanguage>>,
        ISingletonDependency
    {
        /// <summary>
        ///     Cache name for languages.
        /// </summary>
        public const string CacheName = "KontecgBaselineLanguages";

        private readonly ICacheManager _cacheManager;

        private readonly IRepository<ApplicationLanguage> _languageRepository;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationLanguageManager" /> class.
        /// </summary>
        public ApplicationLanguageManager(
            IRepository<ApplicationLanguage> languageRepository,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager)
        {
            _languageRepository = languageRepository;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
        }

        private ITypedCache<int, Dictionary<string, ApplicationLanguage>> LanguageListCache =>
            _cacheManager.GetCache<int, Dictionary<string, ApplicationLanguage>>(CacheName);

        /// <summary>
        ///     Gets list of all languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        public virtual async Task<IReadOnlyList<ApplicationLanguage>> GetLanguagesAsync(int? companyId)
        {
            return (await GetLanguageDictionaryAsync(companyId)).Values.ToImmutableList();
        }

        public virtual async Task<IReadOnlyList<ApplicationLanguage>> GetActiveLanguagesAsync(int? companyId)
        {
            return (await GetLanguagesAsync(companyId)).Where(l => !l.IsDisabled).ToImmutableList();
        }

        /// <summary>
        ///     Gets list of all languages available to given company (or null for host)
        /// </summary>
        /// <param name="companyId">CompanyId or null for host</param>
        public virtual IReadOnlyList<ApplicationLanguage> GetLanguages(int? companyId)
        {
            return GetLanguageDictionary(companyId).Values.ToImmutableList();
        }

        public virtual IReadOnlyList<ApplicationLanguage> GetActiveLanguages(int? companyId)
        {
            return GetLanguages(companyId).Where(l => !l.IsDisabled).ToImmutableList();
        }

        /// <summary>
        ///     Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        public virtual async Task AddAsync(ApplicationLanguage language)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if ((await GetLanguagesAsync(language.CompanyId)).Any(l => l.Name == language.Name))
                {
                    throw new KontecgException("There is already a language with name = " + language.Name);
                }

                using (_unitOfWorkManager.Current.SetCompanyId(language.CompanyId))
                {
                    await _languageRepository.InsertAsync(language);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        /// <summary>
        ///     Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        public virtual void Add(ApplicationLanguage language)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (GetLanguages(language.CompanyId).Any(l => l.Name == language.Name))
                {
                    throw new KontecgException("There is already a language with name = " + language.Name);
                }

                using (_unitOfWorkManager.Current.SetCompanyId(language.CompanyId))
                {
                    _languageRepository.Insert(language);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        /// <summary>
        ///     Deletes a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual async Task RemoveAsync(int? companyId, string languageName)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                ApplicationLanguage currentLanguage =
                    (await GetLanguagesAsync(companyId)).FirstOrDefault(l => l.Name == languageName);
                if (currentLanguage == null)
                {
                    return;
                }

                if (currentLanguage.CompanyId == null && companyId != null)
                {
                    throw new KontecgException("Can not delete a host language from company!");
                }

                using (_unitOfWorkManager.Current.SetCompanyId(currentLanguage.CompanyId))
                {
                    await _languageRepository.DeleteAsync(currentLanguage.Id);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        /// <summary>
        ///     Deletes a language.
        /// </summary>
        /// <param name="companyId">Company Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual void Remove(int? companyId, string languageName)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                ApplicationLanguage currentLanguage =
                    GetLanguages(companyId).FirstOrDefault(l => l.Name == languageName);
                if (currentLanguage == null)
                {
                    return;
                }

                if (currentLanguage.CompanyId == null && companyId != null)
                {
                    throw new KontecgException("Can not delete a host language from company!");
                }

                using (_unitOfWorkManager.Current.SetCompanyId(currentLanguage.CompanyId))
                {
                    _languageRepository.Delete(currentLanguage.Id);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        /// <summary>
        ///     Updates a language.
        /// </summary>
        public virtual async Task UpdateAsync(int? companyId, ApplicationLanguage language)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                ApplicationLanguage existingLanguageWithSameName =
                    (await GetLanguagesAsync(language.CompanyId)).FirstOrDefault(l => l.Name == language.Name);
                if (existingLanguageWithSameName != null)
                {
                    if (existingLanguageWithSameName.Id != language.Id)
                    {
                        throw new KontecgException("There is already a language with name = " + language.Name);
                    }
                }

                if (language.CompanyId == null && companyId != null)
                {
                    throw new KontecgException("Can not update a host language from company");
                }

                using (_unitOfWorkManager.Current.SetCompanyId(language.CompanyId))
                {
                    await _languageRepository.UpdateAsync(language);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        /// <summary>
        ///     Updates a language.
        /// </summary>
        public virtual void Update(int? companyId, ApplicationLanguage language)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                ApplicationLanguage existingLanguageWithSameName =
                    GetLanguages(language.CompanyId).FirstOrDefault(l => l.Name == language.Name);
                if (existingLanguageWithSameName != null)
                {
                    if (existingLanguageWithSameName.Id != language.Id)
                    {
                        throw new KontecgException("There is already a language with name = " + language.Name);
                    }
                }

                if (language.CompanyId == null && companyId != null)
                {
                    throw new KontecgException("Can not update a host language from company");
                }

                using (_unitOfWorkManager.Current.SetCompanyId(language.CompanyId))
                {
                    _languageRepository.Update(language);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        /// <summary>
        ///     Gets the default language or null for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        public virtual async Task<ApplicationLanguage> GetDefaultLanguageOrNullAsync(int? companyId)
        {
            string defaultLanguageName = companyId.HasValue
                ? await _settingManager.GetSettingValueForCompanyAsync(LocalizationSettingNames.DefaultLanguage,
                    companyId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync(LocalizationSettingNames.DefaultLanguage);

            return (await GetLanguagesAsync(companyId)).FirstOrDefault(l => l.Name == defaultLanguageName);
        }

        /// <summary>
        ///     Gets the default language or null for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        public virtual ApplicationLanguage GetDefaultLanguageOrNull(int? companyId)
        {
            string defaultLanguageName = companyId.HasValue
                ? _settingManager.GetSettingValueForCompany(LocalizationSettingNames.DefaultLanguage, companyId.Value)
                : _settingManager.GetSettingValueForApplication(LocalizationSettingNames.DefaultLanguage);

            return GetLanguages(companyId).FirstOrDefault(l => l.Name == defaultLanguageName);
        }

        /// <summary>
        ///     Sets the default language for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual async Task SetDefaultLanguageAsync(int? companyId, string languageName)
        {
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageName);
            if (companyId.HasValue)
            {
                await _settingManager.ChangeSettingForCompanyAsync(companyId.Value,
                    LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
            else
            {
                await _settingManager.ChangeSettingForApplicationAsync(LocalizationSettingNames.DefaultLanguage,
                    cultureInfo.Name);
            }
        }

        /// <summary>
        ///     Sets the default language for a company or the host.
        /// </summary>
        /// <param name="companyId">Company Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual void SetDefaultLanguage(int? companyId, string languageName)
        {
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageName);
            if (companyId.HasValue)
            {
                _settingManager.ChangeSettingForCompany(companyId.Value, LocalizationSettingNames.DefaultLanguage,
                    cultureInfo.Name);
            }
            else
            {
                _settingManager.ChangeSettingForApplication(LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguage> eventData)
        {
            LanguageListCache.Remove(eventData.Entity.CompanyId ?? 0);

            //Also invalidate the language script cache
            _cacheManager.GetCache("KontecgLocalizationScripts").Clear();
        }

        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionaryAsync(int? companyId)
        {
            //Creates a copy of the cached dictionary (to not modify it)
            Dictionary<string, ApplicationLanguage> languageDictionary =
                new Dictionary<string, ApplicationLanguage>(await GetLanguageDictionaryFromCacheAsync(null));

            if (companyId == null)
            {
                return languageDictionary;
            }

            //Override company languages
            foreach (KeyValuePair<string, ApplicationLanguage> companyLanguage in
                     await GetLanguageDictionaryFromCacheAsync(companyId.Value))
            {
                languageDictionary[companyLanguage.Key] = companyLanguage.Value;
            }

            return languageDictionary;
        }

        protected virtual Dictionary<string, ApplicationLanguage> GetLanguageDictionary(int? companyId)
        {
            //Creates a copy of the cached dictionary (to not modify it)
            Dictionary<string, ApplicationLanguage> languageDictionary =
                new Dictionary<string, ApplicationLanguage>(GetLanguageDictionaryFromCache(null));

            if (companyId == null)
            {
                return languageDictionary;
            }

            //Override company languages
            foreach (KeyValuePair<string, ApplicationLanguage> companyLanguage in GetLanguageDictionaryFromCache(
                         companyId.Value))
            {
                languageDictionary[companyLanguage.Key] = companyLanguage.Value;
            }

            return languageDictionary;
        }

        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguagesFromDatabaseAsync(
            int? companyId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    return (await _languageRepository.GetAllListAsync()).ToDictionary(l => l.Name);
                }
            });
        }

        protected virtual Dictionary<string, ApplicationLanguage> GetLanguagesFromDatabase(int? companyId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    return _languageRepository.GetAllList().ToDictionary(l => l.Name);
                }
            });
        }

        private Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionaryFromCacheAsync(int? companyId)
        {
            return LanguageListCache.GetAsync(companyId ?? 0, () => GetLanguagesFromDatabaseAsync(companyId));
        }

        private Dictionary<string, ApplicationLanguage> GetLanguageDictionaryFromCache(int? companyId)
        {
            return LanguageListCache.Get(companyId ?? 0, () => GetLanguagesFromDatabase(companyId));
        }
    }
}
