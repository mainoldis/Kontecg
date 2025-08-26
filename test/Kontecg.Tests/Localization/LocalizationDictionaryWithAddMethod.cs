﻿using System.Globalization;
using Kontecg.Localization.Dictionaries;

namespace Kontecg.Tests.Localization
{
    /// <summary>
    /// Derived from LocalizationDictionary to be able to add "<see cref="Add"/>" method.
    /// </summary>
    public class LocalizationDictionaryWithAddMethod : LocalizationDictionary
    {
        public LocalizationDictionaryWithAddMethod(CultureInfo cultureInfo)
            : base(cultureInfo)
        {
        }

        public void Add(string name, string value)
        {
            this[name] = value;
        }
    }
}
