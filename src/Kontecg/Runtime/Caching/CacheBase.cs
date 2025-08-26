namespace Kontecg.Runtime.Caching
{
    /// <summary>
    ///     Base class for caches.
    ///     It's used to simplify implementing <see cref="ICache" />.
    /// </summary>
    public abstract class CacheBase : KontecgCacheBase<string, object>, ICache
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name) : base(name)
        {
        }
    }
}
