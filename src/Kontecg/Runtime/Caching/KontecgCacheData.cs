using Kontecg.Json;
using Kontecg.Reflection;

namespace Kontecg.Runtime.Caching
{
    /// <summary>
    ///     A class to hold the Type information and Serialized payload for data stored in the cache.
    /// </summary>
    public class KontecgCacheData
    {
        public KontecgCacheData(
            string type, string payload)
        {
            Type = type;
            Payload = payload;
        }

        public string Payload { get; set; }

        public string Type { get; set; }

        public static KontecgCacheData Deserialize(string serializedCacheData) => serializedCacheData.FromJsonString<KontecgCacheData>();

        public static KontecgCacheData Serialize(object obj, bool withAssemblyName = true)
        {
            return new KontecgCacheData(
                TypeHelper.SerializeType(obj.GetType(), withAssemblyName).ToString(),
                obj.ToJsonString());
        }
    }
}
