using Kontecg.Dependency;
using Kontecg.Runtime.Session;
using Microsoft.Extensions.Options;

namespace Kontecg.Runtime.Caching.Redis
{
    public class KontecgRedisCacheKeyNormalizer : IKontecgRedisCacheKeyNormalizer, ITransientDependency
    {
        public IKontecgSession KontecgSession { get; set; }
        protected KontecgRedisCacheOptions RedisCacheOptions { get; }

        public KontecgRedisCacheKeyNormalizer(
        IOptions<KontecgRedisCacheOptions> redisCacheOptions)
        {
            KontecgSession = NullKontecgSession.Instance;
            RedisCacheOptions = redisCacheOptions.Value;
        }

        public string NormalizeKey(KontecgRedisCacheKeyNormalizeArgs args)
        {
            var normalizedKey = $"n:{args.CacheName},c:{RedisCacheOptions.KeyPrefix}{args.Key}";

            if (args.MultiCompanyEnabled && KontecgSession.CompanyId != null && RedisCacheOptions.CompanyKeyEnabled)
            {
                normalizedKey = $"t:{KontecgSession.CompanyId},{normalizedKey}";
            }

            return normalizedKey;
        }
    }
}
