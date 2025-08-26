using System.ComponentModel;

namespace Kontecg.Localization
{
    public class KontecgDisplayNameAttribute : DisplayNameAttribute
    {
        public KontecgDisplayNameAttribute(string sourceName, string key)
        {
            SourceName = sourceName;
            Key = key;
        }

        public override string DisplayName => LocalizationHelper.GetString(SourceName, Key);

        public string SourceName { get; set; }

        public string Key { get; set; }
    }
}
