using StackExchange.Redis;

namespace Kontecg.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to get <see cref="IDatabase"/> for Redis cache.
    /// </summary>
    public interface IKontecgRedisCacheDatabaseProvider 
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
