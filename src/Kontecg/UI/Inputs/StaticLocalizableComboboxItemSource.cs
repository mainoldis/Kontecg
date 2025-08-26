using System;
using System.Collections.Generic;

namespace Kontecg.UI.Inputs
{
    [Serializable]
    public class StaticLocalizableComboboxItemSource : ILocalizableComboboxItemSource
    {
        public StaticLocalizableComboboxItemSource(params ILocalizableComboboxItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items.Length <= 0)
            {
                throw new ArgumentException("Items can not be empty!");
            }

            Items = items;
        }

        public ICollection<ILocalizableComboboxItem> Items { get; private set; }
    }
}
