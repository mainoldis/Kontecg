using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Localization.Dictionaries;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Implements <see cref="IMultiCompanyLocalizationDictionary" />.
    /// </summary>
    public class MultiCompanyLocalizationDictionary :
        IMultiCompanyLocalizationDictionary
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ApplicationLanguageText, long> _customLocalizationRepository;
        private readonly ILocalizationDictionary _internalDictionary;
        private readonly IKontecgSession _session;
        private readonly string _sourceName;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiCompanyLocalizationDictionary" /> class.
        /// </summary>
        public MultiCompanyLocalizationDictionary(
            string sourceName,
            ILocalizationDictionary internalDictionary,
            IRepository<ApplicationLanguageText, long> customLocalizationRepository,
            ICacheManager cacheManager,
            IKontecgSession session,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _sourceName = sourceName;
            _internalDictionary = internalDictionary;
            _customLocalizationRepository = customLocalizationRepository;
            _cacheManager = cacheManager;
            _session = session;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public CultureInfo CultureInfo => _internalDictionary.CultureInfo;

        public string this[string name]
        {
            get => _internalDictionary[name];
            set => _internalDictionary[name] = value;
        }

        public LocalizedString GetOrNull(string name)
        {
            return GetOrNull(_session.CompanyId, name);
        }

        public IReadOnlyList<LocalizedString> GetStringsOrNull(List<string> names)
        {
            return GetStringsOrNull(_session.CompanyId, names);
        }

        public LocalizedString GetOrNull(int? companyId, string name)
        {
            //Get cache
            ITypedCache<string, Dictionary<string, string>> cache =
                _cacheManager.GetMultiCompanyLocalizationDictionaryCache();

            //Get for current company
            Dictionary<string, string> dictionary =
                cache.Get(CalculateCacheKey(companyId), () => GetAllValuesFromDatabase(companyId));
            string value = dictionary.GetOrDefault(name);
            if (value != null)
            {
                return new LocalizedString(name, value, CultureInfo);
            }

            //Fall back to host
            if (companyId != null)
            {
                dictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                value = dictionary.GetOrDefault(name);
                if (value != null)
                {
                    return new LocalizedString(name, value, CultureInfo);
                }
            }

            //Not found in database, fall back to internal dictionary
            LocalizedString internalLocalizedString = _internalDictionary.GetOrNull(name);
            return internalLocalizedString;

            //Not found at all
        }

        public IReadOnlyList<LocalizedString> GetStringsOrNull(int? companyId, List<string> names)
        {
            //Get cache
            ITypedCache<string, Dictionary<string, string>> cache =
                _cacheManager.GetMultiCompanyLocalizationDictionaryCache();

            //Create a temp dictionary to build (by underlying dictionary)
            Dictionary<string, LocalizedString> dictionary = new Dictionary<string, LocalizedString>();

            foreach (LocalizedString localizedString in _internalDictionary.GetStringsOrNull(names))
            {
                dictionary[localizedString.Name] = localizedString;
            }

            //Override by host
            if (companyId != null)
            {
                Dictionary<string, string> defaultDictionary =
                    cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foreach (KeyValuePair<string, string> keyValue in defaultDictionary.Where(x => names.Contains(x.Key)))
                {
                    dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
                }
            }

            //Override by company
            Dictionary<string, string> companyDictionary =
                cache.Get(CalculateCacheKey(companyId), () => GetAllValuesFromDatabase(companyId));
            foreach (KeyValuePair<string, string> keyValue in companyDictionary.Where(x => names.Contains(x.Key)))
            {
                dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
            }

            return dictionary.Values.ToImmutableList();
        }


        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(_session.CompanyId);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(int? companyId)
        {
            //Get cache
            ITypedCache<string, Dictionary<string, string>> cache =
                _cacheManager.GetMultiCompanyLocalizationDictionaryCache();

            //Create a temp dictionary to build (by underlying dictionary)
            Dictionary<string, LocalizedString> dictionary = new Dictionary<string, LocalizedString>();

            foreach (LocalizedString localizedString in _internalDictionary.GetAllStrings())
            {
                dictionary[localizedString.Name] = localizedString;
            }

            //Override by host
            if (companyId != null)
            {
                Dictionary<string, string> defaultDictionary =
                    cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foreach (KeyValuePair<string, string> keyValue in defaultDictionary)
                {
                    dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
                }
            }

            //Override by company
            Dictionary<string, string> companyDictionary =
                cache.Get(CalculateCacheKey(companyId), () => GetAllValuesFromDatabase(companyId));
            foreach (KeyValuePair<string, string> keyValue in companyDictionary)
            {
                dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
            }

            return dictionary.Values.ToImmutableList();
        }

        protected virtual Dictionary<string, string> GetAllValuesFromDatabase(int? companyId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    return _customLocalizationRepository
                        .GetAllList(l => l.Source == _sourceName && l.LanguageName == CultureInfo.Name)
                        .ToDictionary(l => l.Key, l => l.Value);
                }
            });
        }

        private string CalculateCacheKey(int? companyId)
        {
            return MultiCompanyLocalizationDictionaryCacheHelper.CalculateCacheKey(
                companyId,
                _sourceName,
                CultureInfo.Name
            );
        }
    }
}
