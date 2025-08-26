using System.Collections.Generic;

namespace Kontecg.UI.Inputs
{
    public interface ILocalizableComboboxItemSource
    {
        ICollection<ILocalizableComboboxItem> Items { get; }
    }
}
