using System.Collections.Generic;
using Kontecg.Localization.Sources;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines a specialized list to store <see cref="ILocalizationSource" /> object.
    /// </summary>
    public interface ILocalizationSourceList : IList<ILocalizationSource>
    {
        /// <summary>
        ///     Extensions for dictionary based localization sources.
        /// </summary>
        IList<LocalizationSourceExtensionInfo> Extensions { get; }
    }
}
