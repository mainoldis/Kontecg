using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Localization
{
    public class MultiCompanyLocalizationSource : DictionaryBasedLocalizationSource, IMultiCompanyLocalizationSource
    {
        public MultiCompanyLocalizationSource(string name,
            MultiCompanyLocalizationDictionaryProvider dictionaryProvider)
            : base(name, dictionaryProvider)
        {
            Logger = NullLogger.Instance;
        }

        public new MultiCompanyLocalizationDictionaryProvider DictionaryProvider =>
            base.DictionaryProvider.As<MultiCompanyLocalizationDictionaryProvider>();

        public ILogger Logger { get; set; }

        public override void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            base.Initialize(configuration, iocResolver);

            if (Logger is NullLogger && iocResolver.IsRegistered(typeof(ILoggerFactory)))
            {
                Logger = iocResolver.Resolve<ILoggerFactory>().Create(typeof(MultiCompanyLocalizationSource));
            }
        }

        public string GetString(int? companyId, string name, CultureInfo culture)
        {
            string value = GetStringOrNull(companyId, name, culture);

            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name, culture);
            }

            return value;
        }

        public string GetStringOrNull(int? companyId, string name, CultureInfo culture, bool tryDefaults = true)
        {
            string cultureName = culture.Name;
            IDictionary<string, ILocalizationDictionary> dictionaries = DictionaryProvider.Dictionaries;

            //Try to get from original dictionary (with country code)
            if (dictionaries.TryGetValue(cultureName, out ILocalizationDictionary originalDictionary))
            {
                LocalizedString strOriginal = originalDictionary
                    .As<IMultiCompanyLocalizationDictionary>()
                    .GetOrNull(companyId, name);

                if (strOriginal != null)
                {
                    return strOriginal.Value;
                }
            }

            if (!tryDefaults)
            {
                return null;
            }

            //Try to get from same language dictionary (without country code)
            if (cultureName.Contains("-")) //Example: "tr-TR" (length=5)
            {
                if (dictionaries.TryGetValue(GetBaseCultureName(cultureName),
                        out ILocalizationDictionary langDictionary))
                {
                    LocalizedString strLang = langDictionary.As<IMultiCompanyLocalizationDictionary>()
                        .GetOrNull(companyId, name);
                    if (strLang != null)
                    {
                        return strLang.Value;
                    }
                }
            }

            //Try to get from default language
            ILocalizationDictionary defaultDictionary = DictionaryProvider.DefaultDictionary;

            LocalizedString strDefault =
                defaultDictionary?.As<IMultiCompanyLocalizationDictionary>().GetOrNull(companyId, name);

            return strDefault?.Value;
        }

        public List<string> GetStrings(int? companyId, List<string> names, CultureInfo culture)
        {
            List<string> value = GetStringsOrNull(companyId, names, culture);

            if (value == null)
            {
                return ReturnGivenNamesOrThrowException(names, culture);
            }

            return value;
        }

        public List<string> GetStringsOrNull(int? companyId, List<string> names, CultureInfo culture,
            bool tryDefaults = true)
        {
            string cultureName = culture.Name;
            IDictionary<string, ILocalizationDictionary> dictionaries = DictionaryProvider.Dictionaries;

            //Try to get from original dictionary (with country code)
            if (dictionaries.TryGetValue(cultureName, out ILocalizationDictionary originalDictionary))
            {
                IReadOnlyList<LocalizedString> strOriginal = originalDictionary
                    .As<IMultiCompanyLocalizationDictionary>()
                    .GetStringsOrNull(companyId, names);

                if (!strOriginal.IsNullOrEmpty())
                {
                    return strOriginal.Select(x => x.Value).ToList();
                }
            }

            if (!tryDefaults)
            {
                return null;
            }

            //Try to get from same language dictionary (without country code)
            if (cultureName.Contains("-")) //Example: "tr-TR" (length=5)
            {
                if (dictionaries.TryGetValue(GetBaseCultureName(cultureName),
                        out ILocalizationDictionary langDictionary))
                {
                    IReadOnlyList<LocalizedString> strLang = langDictionary.As<IMultiCompanyLocalizationDictionary>()
                        .GetStringsOrNull(companyId, names);
                    if (!strLang.IsNullOrEmpty())
                    {
                        return strLang.Select(x => x.Value).ToList();
                    }
                }
            }

            //Try to get from default language
            ILocalizationDictionary defaultDictionary = DictionaryProvider.DefaultDictionary;
            if (defaultDictionary == null)
            {
                return null;
            }

            IReadOnlyList<LocalizedString> strDefault = defaultDictionary.As<IMultiCompanyLocalizationDictionary>()
                .GetStringsOrNull(companyId, names);
            if (strDefault.IsNullOrEmpty())
            {
                return null;
            }

            return strDefault.Select(x => x.Value).ToList();
        }

        public override void Extend(ILocalizationDictionary dictionary)
        {
            DictionaryProvider.Extend(dictionary);
        }

        private static string GetBaseCultureName(string cultureName)
        {
            return cultureName.Contains("-")
                ? cultureName.Left(cultureName.IndexOf("-", StringComparison.Ordinal))
                : cultureName;
        }
    }
}
