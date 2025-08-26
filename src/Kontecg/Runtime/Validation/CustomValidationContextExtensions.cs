using Kontecg.Localization;
using Kontecg.Localization.Sources;

namespace Kontecg.Runtime.Validation
{
    public static class CustomValidationContextExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <param name="sourceName">Localization source name</param>
        /// <param name="key">Localization key</param>
        public static string Localize(this CustomValidationContext validationContext, string sourceName, string key)
        {
            ILocalizationManager localizationManager = validationContext.IocResolver.Resolve<ILocalizationManager>();
            ILocalizationSource source = localizationManager.GetSource(sourceName);
            return source.GetString(key);
        }
    }
}
