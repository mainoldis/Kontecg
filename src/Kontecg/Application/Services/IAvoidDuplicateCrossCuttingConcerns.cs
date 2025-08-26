using System.Collections.Generic;

namespace Kontecg.Application.Services
{
    /// <summary>
    /// Defines a contract to avoid applying duplicate cross-cutting concerns.
    /// </summary>
    public interface IAvoidDuplicateCrossCuttingConcerns
    {
        /// <summary>
        /// Gets the list of cross-cutting concerns that have already been applied.
        /// </summary>
        List<string> AppliedCrossCuttingConcerns { get; }
    }
}
