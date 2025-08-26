using System.Collections.Generic;
using Kontecg.Localization.Sources;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     A specialized list to store <see cref="ILocalizationSource" /> object.
    /// </summary>
    internal class LocalizationSourceList : List<ILocalizationSource>, ILocalizationSourceList
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public LocalizationSourceList()
        {
            Extensions = new List<LocalizationSourceExtensionInfo>();
        }

        public IList<LocalizationSourceExtensionInfo> Extensions { get; }
    }
}
