using Kontecg.Runtime.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Runtime.Caching.Memory
{
    public class KontecgMemoryCache_Tests : TestBaseWithLocalIocManager
    {
        private readonly KontecgMemoryCache _memoryCache;

        public KontecgMemoryCache_Tests()
        {
            _memoryCache = new KontecgMemoryCache("test cache", new MemoryCacheOptions());
        }

        [Fact]
        public void Single_Key_Get_Test()
        {
            var cacheValue = _memoryCache.GetOrDefault("A");
            cacheValue.ShouldBeNull();

            cacheValue = _memoryCache.Get("A", (key) => "test");
            cacheValue.ShouldBe("test");
        }

        [Fact]
        public void Multi_Keys_Get_Test()
        {
            var cacheValues = _memoryCache.GetOrDefault(new[] { "A", "B" });
            cacheValues.ShouldNotBeNull();
            cacheValues.Length.ShouldBe(2);
            cacheValues[0].ShouldBeNull();
            cacheValues[1].ShouldBeNull();

            cacheValues = _memoryCache.Get(new[] { "A", "B" }, (key) => "test " + key);
            cacheValues.ShouldNotBeNull();
            cacheValues.Length.ShouldBe(2);
            cacheValues[0].ShouldBe("test A");
            cacheValues[1].ShouldBe("test B");
        }
    }
}
