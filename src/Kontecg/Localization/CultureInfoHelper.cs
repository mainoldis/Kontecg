using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Kontecg.Localization
{
    public static class CultureInfoHelper
    {
        public static IDisposable Use([NotNull] string culture, string uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            return Use(CultureInfo.GetCultureInfo(culture),
                uiCulture == null ? null : CultureInfo.GetCultureInfo(uiCulture));
        }

        public static IDisposable Use([NotNull] CultureInfo culture, CultureInfo uiCulture = null)
        {
            Check.NotNull(culture, nameof(culture));

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            CultureInfo currentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture ?? culture;

            return new DisposeAction(() =>
            {
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentUiCulture;
            });
        }
    }
}
