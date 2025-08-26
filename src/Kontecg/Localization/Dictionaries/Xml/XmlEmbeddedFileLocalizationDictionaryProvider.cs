using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kontecg.Localization.Dictionaries.Xml
{
    /// <summary>
    ///     Provides localization dictionaries from XML files embedded into an <see cref="Assembly" />.
    /// </summary>
    public class XmlEmbeddedFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly Assembly _assembly;
        private readonly string _rootNamespace;

        /// <summary>
        ///     Creates a new <see cref="XmlEmbeddedFileLocalizationDictionaryProvider" /> object.
        /// </summary>
        /// <param name="assembly">Assembly that contains embedded xml files</param>
        /// <param name="rootNamespace">Namespace of the embedded xml dictionary files</param>
        public XmlEmbeddedFileLocalizationDictionaryProvider(Assembly assembly, string rootNamespace)
        {
            _assembly = assembly;
            _rootNamespace = rootNamespace;
        }

        protected override void InitializeDictionaries()
        {
            CultureInfo[] allCultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            List<string> resourceNames = _assembly.GetManifestResourceNames().Where(resourceName =>
                allCultureInfos.Any(culture => resourceName.EndsWith($"{SourceName}.xml", true, null) ||
                                               resourceName.EndsWith($"{SourceName}-{culture.Name}.xml", true,
                                                   null))).ToList();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith(_rootNamespace))
                {
                    using Stream stream = _assembly.GetManifestResourceStream(resourceName);
                    string xmlString = Utf8Helper.ReadStringFromStream(stream);
                    InitializeDictionary(CreateXmlLocalizationDictionary(xmlString),
                        resourceName.EndsWith(SourceName + ".xml"));
                }
            }
        }

        protected virtual XmlLocalizationDictionary CreateXmlLocalizationDictionary(string xmlString)
        {
            return XmlLocalizationDictionary.BuildFomXmlString(xmlString);
        }
    }
}
