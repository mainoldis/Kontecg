using System;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Scopes of a <see cref="Feature" />.
    /// </summary>
    [Flags]
    public enum FeatureScopes
    {
        /// <summary>
        ///     This <see cref="Feature" /> can be enabled/disabled per client.
        /// </summary>
        Client = 1,

        /// <summary>
        ///     This Feature<see cref="Feature" /> can be enabled/disabled per company.
        /// </summary>
        Company = 2,

        /// <summary>
        ///     This <see cref="Feature" /> can be enabled/disabled per company and client.
        /// </summary>
        All = 3
    }
}
