using Kontecg.Localization.Dictionaries.Xml;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Localization
{
    public class XmlLocalizationDictionaryBuilder_Tests
    {
        [Fact]
        public void Can_Build_LocalizationDictionary_From_Xml_String()
        {
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<localizationDictionary culture=""tr"">
  <texts>
    <text name=""hello"" value=""Merhaba"" />
    <text name=""world"">D�nya</text>
  </texts>
</localizationDictionary>";

            var dictionary = XmlLocalizationDictionary.BuildFomXmlString(xmlString);

            dictionary.CultureInfo.Name.ShouldBe("tr");
            dictionary["hello"].ShouldBe("Merhaba");
            dictionary["world"].ShouldBe("D�nya");
        }

        [Fact]
        public void Should_Throw_Exception_For_Duplicate_Name()
        {
            var xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<localizationDictionary culture=""tr"">
  <texts>
    <text name=""hello"" value=""Merhaba"" />
    <text name=""hello"" value=""Merhabalar""></text>
  </texts>
</localizationDictionary>";

            Assert.Throws<KontecgException>(() => XmlLocalizationDictionary.BuildFomXmlString(xmlString));
        }
    }
}
