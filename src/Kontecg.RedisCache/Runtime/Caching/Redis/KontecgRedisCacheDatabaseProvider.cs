using System;
using Kontecg.Dependency;
using StackExchange.Redis;

namespace Kontecg.Runtime.Caching.Redis
{
    /// <summary>
    /// Implements <see cref="IKontecgRedisCacheDatabaseProvider"/>.
    /// </summary>
    public class KontecgRedisCacheDatabaseProvider : IKontecgRedisCacheDatabaseProvider, ISingletonDependency
    {
        private readonly KontecgRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KontecgRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public KontecgRedisCacheDatabaseProvider(KontecgRedisCacheOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
    }
}
