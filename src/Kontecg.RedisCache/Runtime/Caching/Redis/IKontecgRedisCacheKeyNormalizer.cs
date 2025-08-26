namespace Kontecg.Runtime.Caching.Redis
{
    public interface IKontecgRedisCacheKeyNormalizer
    {
        string NormalizeKey(KontecgRedisCacheKeyNormalizeArgs args);
    }
}
