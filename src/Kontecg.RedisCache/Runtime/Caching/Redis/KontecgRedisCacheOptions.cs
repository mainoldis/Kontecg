using System.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Extensions;

namespace Kontecg.Runtime.Caching.Redis
{
    public class KontecgRedisCacheOptions
    {
        public IKontecgStartupConfiguration KontecgStartupConfiguration { get; }

        private const string ConnectionStringKey = "Kontecg.Redis.Cache";

        private const string DatabaseIdSettingKey = "Kontecg.Redis.Cache.DatabaseId";

        public string ConnectionString { get; set; }

        public int DatabaseId { get; set; }

        public readonly string OnlineClientsStoreKey = "Kontecg.RealTime.OnlineClients";

        public string KeyPrefix { get; set; }

        public bool CompanyKeyEnabled { get; set; }

        /// <summary>
        /// Required for serialization
        /// </summary>
        public KontecgRedisCacheOptions()
        {
            
        }
        
        public KontecgRedisCacheOptions(IKontecgStartupConfiguration kontecgStartupConfiguration)
        {
            KontecgStartupConfiguration = kontecgStartupConfiguration;

            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
            KeyPrefix = "";
            CompanyKeyEnabled = false;
        }

        private static int GetDefaultDatabaseId()
        {
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
        }

        private static string GetDefaultConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
        }

    }
}
