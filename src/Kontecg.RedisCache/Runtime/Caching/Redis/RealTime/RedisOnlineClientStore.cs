using Kontecg.Dependency;
using Kontecg.RealTime;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Kontecg.Runtime.Caching.Redis.RealTime
{
    public class RedisOnlineClientStore : IOnlineClientStore, ISingletonDependency
    {
        private readonly IKontecgRedisCacheDatabaseProvider _database;

        private readonly string _clientStoreKey;
        private readonly string _userStoreKey;

        public RedisOnlineClientStore(
            IKontecgRedisCacheDatabaseProvider database,
            KontecgRedisCacheOptions options)
        {
            _database = database;

            _clientStoreKey = options.OnlineClientsStoreKey + ".Clients";
            _userStoreKey = options.OnlineClientsStoreKey + ".Users";
        }

        public void Add(IOnlineClient client)
        {
            var database = GetDatabase();
            database.HashSet(_clientStoreKey, new[]
            {
                new HashEntry(client.ConnectionId, client.ToString())
            });
        }

        public async Task AddAsync(IOnlineClient client)
        {
            var database = GetDatabase();
            await database.HashSetAsync(_clientStoreKey, new[]
            {
                new HashEntry(client.ConnectionId, client.ToString())
            });
        }

        public bool Remove(string connectionId)
        {
            var database = GetDatabase();

            var clientValue = database.HashGet(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                return true;
            }

            database.HashDelete(_clientStoreKey, connectionId);
            return true;
        }

        public async Task<bool> RemoveAsync(string connectionId)
        {
            var database = GetDatabase();

            var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                return true;
            }

            await database.HashDeleteAsync(_clientStoreKey, connectionId);
            return true;
        }

        public bool TryRemove(string connectionId, Action<IOnlineClient> clientAction)
        {
            try
            {
                var database = GetDatabase();

                var clientValue = database.HashGet(_clientStoreKey, connectionId);
                if (clientValue.IsNullOrEmpty)
                {
                    clientAction(null);
                    return true;
                }

                clientAction(JsonConvert.DeserializeObject<OnlineClient>(clientValue));

                database.HashDelete(_clientStoreKey, connectionId);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                clientAction(null);
                return false;
            }
        }

        public async Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            try
            {
                var database = GetDatabase();

                var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
                if (clientValue.IsNullOrEmpty)
                {
                    clientAction(null);
                    return true;
                }

                clientAction(JsonConvert.DeserializeObject<OnlineClient>(clientValue));

                await database.HashDeleteAsync(_clientStoreKey, connectionId);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                clientAction(null);
                return false;
            }
        }

        public bool TryGet(string connectionId, Action<IOnlineClient> clientAction)
        {
            var database = GetDatabase();
            var clientValue = database.HashGet(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                clientAction(null);
                return false;
            }

            clientAction(JsonConvert.DeserializeObject<OnlineClient>(clientValue));
            return true;
        }

        public async Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            var database = GetDatabase();
            var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                clientAction(null);
                return false;
            }

            clientAction(JsonConvert.DeserializeObject<OnlineClient>(clientValue));
            return true;
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            var database = GetDatabase();
            var clientsEntries = database.HashGetAll(_clientStoreKey);
            var clients = clientsEntries
                .Select(entry => JsonConvert.DeserializeObject<OnlineClient>(entry.Value))
                .Cast<IOnlineClient>()
                .ToList();

            return clients.ToImmutableList();
        }

        public async Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
        {
            var database = GetDatabase();
            var clientsEntries = await database.HashGetAllAsync(_clientStoreKey);
            var clients = clientsEntries
                .Select(entry => JsonConvert.DeserializeObject<OnlineClient>(entry.Value))
                .Cast<IOnlineClient>()
                .ToList();

            return clients.ToImmutableList();
        }

        public IReadOnlyList<IOnlineClient> GetAllByUserId(UserIdentifier userIdentifier)
        {
            var database = GetDatabase();
            var clientsEntries = database.HashGetAll(_clientStoreKey);
            var clients = new List<IOnlineClient>();
            foreach (var entry in clientsEntries)
            {
                clients.Add(JsonConvert.DeserializeObject<OnlineClient>(entry.Value));
            }

            return clients
                .Where(e => e.CompanyId == userIdentifier.CompanyId && e.UserId == userIdentifier.UserId)
                .ToImmutableList();
        }

        public async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
        {
            var database = GetDatabase();
            var clientsEntries = await database.HashGetAllAsync(_clientStoreKey);
            var clients = new List<IOnlineClient>();
            foreach (var entry in clientsEntries)
            {
                clients.Add(JsonConvert.DeserializeObject<OnlineClient>(entry.Value));
            }

            return clients
                .Where(e => e.CompanyId == userIdentifier.CompanyId && e.UserId == userIdentifier.UserId)
                .ToImmutableList();
        }

        private IDatabase GetDatabase()
        {
            return _database.GetDatabase();
        }
    }
}
