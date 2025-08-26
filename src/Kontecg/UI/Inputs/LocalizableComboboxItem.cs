using System;
using Kontecg.Localization;

namespace Kontecg.UI.Inputs
{
    [Serializable]
    public class LocalizableComboboxItem : ILocalizableComboboxItem
    {
        public LocalizableComboboxItem()
        {
        }

        public LocalizableComboboxItem(string value, ILocalizableString displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        public string Value { get; set; }

        public ILocalizableString DisplayText { get; set; }
    }
}
