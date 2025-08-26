using System;
using Kontecg.Dependency;

namespace Kontecg.Updates
{
    /// <summary>
    ///     A update Source is used to obtain packages updates.
    /// </summary>
    public interface IUpdateSource
    {
        /// <summary>
        ///     Unique Name of the source.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
        /// </summary>
        Uri Source { get; }

        /// <summary>
        ///     This method is called by Kontecg before first usage.
        /// </summary>
        void Initialize(IUpdateConfiguration configuration, IIocResolver iocResolver);
    }
}
