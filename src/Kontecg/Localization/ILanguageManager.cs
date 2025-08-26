using System.Collections.Generic;

namespace Kontecg.Localization
{
    public interface ILanguageManager
    {
        LanguageInfo CurrentLanguage { get; }

        IReadOnlyList<LanguageInfo> GetLanguages();

        IReadOnlyList<LanguageInfo> GetActiveLanguages();
    }
}
