namespace Kontecg.Runtime.Caching.Redis
{
    public class KontecgRedisCacheKeyNormalizeArgs
    {
        public string Key { get; }

        public string CacheName { get; }

        public bool MultiCompanyEnabled { get; }

        public KontecgRedisCacheKeyNormalizeArgs(
            string key,
            string cacheName,
            bool multiCompanyEnabled)
        {
            Key = key;
            CacheName = cacheName;
            MultiCompanyEnabled = multiCompanyEnabled;
        }
    }
}
