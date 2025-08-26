using System;

namespace Kontecg.Runtime.Caching
{
    /// <summary>
    ///     Defines a cache that uses <see cref="string" /> as key, <see cref="object" /> as value.
    /// </summary>
    public interface ICache : IDisposable, ICacheOptions, IKontecgCache<string, object>
    {
    }
}
