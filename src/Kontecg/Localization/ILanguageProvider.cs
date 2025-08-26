using System.Collections.Generic;

namespace Kontecg.Localization
{
    public interface ILanguageProvider
    {
        IReadOnlyList<LanguageInfo> GetLanguages();

        IReadOnlyList<LanguageInfo> GetActiveLanguages();
    }
}
