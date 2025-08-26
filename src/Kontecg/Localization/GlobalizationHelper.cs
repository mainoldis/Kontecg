using System.Globalization;
using System.Linq;
using Kontecg.Extensions;

namespace Kontecg.Localization
{
    public static class GlobalizationHelper
    {
        public static bool IsValidCultureCode(string cultureCode)
        {
            if (cultureCode.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Any(e => e.Name.ToLowerInvariant() == cultureCode.ToLowerInvariant());
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
