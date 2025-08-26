using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Kontecg.Collections.Extensions;
using Kontecg.Extensions;
using Kontecg.Xml.Extensions;

namespace Kontecg.Localization.Dictionaries.Xml
{
    /// <summary>
    ///     This class is used to build a localization dictionary from XML.
    /// </summary>
    /// <remarks>
    ///     Use static Build methods to create instance of this class.
    /// </remarks>
    public class XmlLocalizationDictionary : LocalizationDictionary
    {
        /// <summary>
        ///     Private constructor.
        /// </summary>
        /// <param name="cultureInfo">Culture of the dictionary</param>
        private XmlLocalizationDictionary(CultureInfo cultureInfo)
            : base(cultureInfo)
        {
        }

        /// <summary>
        ///     Builds an <see cref="XmlLocalizationDictionary" /> from given file.
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        public static XmlLocalizationDictionary BuildFomFile(string filePath)
        {
            try
            {
                return BuildFomXmlString(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                throw new KontecgException("Invalid localization file format! " + filePath, ex);
            }
        }

        /// <summary>
        ///     Builds an <see cref="XmlLocalizationDictionary" /> from given xml string.
        /// </summary>
        /// <param name="xmlString">XML string</param>
        public static XmlLocalizationDictionary BuildFomXmlString(string xmlString)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);

            XmlNodeList localizationDictionaryNode = xmlDocument.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new KontecgException("A Localization Xml must include localizationDictionary as root node.");
            }

            string cultureName = localizationDictionaryNode[0].GetAttributeValueOrNull("culture");
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new KontecgException("culture is not defined in language XML file!");
            }

            XmlLocalizationDictionary dictionary =
                new XmlLocalizationDictionary(CultureInfo.GetCultureInfo(cultureName));

            List<string> dublicateNames = new List<string>();

            XmlNodeList textNodes = xmlDocument.SelectNodes("/localizationDictionary/texts/text");
            if (textNodes != null)
            {
                foreach (XmlNode node in textNodes)
                {
                    string name = node.GetAttributeValueOrNull("name");
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new KontecgException("name attribute of a text is empty in given xml string.");
                    }

                    if (dictionary.Contains(name))
                    {
                        dublicateNames.Add(name);
                    }

                    dictionary[name] = (node.GetAttributeValueOrNull("value") ?? node.InnerText).NormalizeLineEndings();
                }
            }

            if (dublicateNames.Count > 0)
            {
                throw new KontecgException(
                    "A dictionary can not contain same key twice. There are some duplicated names: " +
                    dublicateNames.JoinAsString(", "));
            }

            return dictionary;
        }
    }
}
