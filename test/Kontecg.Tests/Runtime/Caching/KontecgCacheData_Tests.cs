using System;
using System.Collections.Generic;
using Kontecg.Json;
using Kontecg.Runtime.Caching;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Runtime.Caching
{
    public class KontecgCacheData_Tests
    {
        [Fact]
        public void Serialize_List_Test()
        {
            List<string> source = new List<string>
            {
                "Stranger Things",
                "The OA",
                "Lost in Space"
            };

            var result = KontecgCacheData.Serialize(source);
            result.Type.ShouldStartWith("System.Collections.Generic.List`1[[System.String,");
            result.Payload.ShouldBe("[\"Stranger Things\",\"The OA\",\"Lost in Space\"]");
        }

        [Fact]
        public void Serialize_Class_Test()
        {
            var source = new MyTestClass
            {
                Field1 = 42,
                Field2 = "Stranger Things"
            };

            var result = KontecgCacheData.Serialize(source);
            result.Type.ShouldBe("Kontecg.Tests.Runtime.Caching.KontecgCacheData_Tests+MyTestClass, Kontecg.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            result.Payload.ShouldBe("{\"Field1\":42,\"Field2\":\"Stranger Things\"}");
        }

        [Fact]
        public void Deserialize_List_Test()
        {
            var json = "{\"Payload\":\"[\\\"Stranger Things\\\",\\\"The OA\\\",\\\"Lost in Space\\\"]\",\"Type\":\"System.Collections.Generic.List`1[[System.String]]\"}";
            var cacheData = KontecgCacheData.Deserialize(json);

            cacheData.ShouldNotBeNull();
        }

        [Fact]
        public void Deserialize_Class_Test()
        {
            var json = "{\"Payload\": \"{\\\"Field1\\\": 42,\\\"Field2\\\":\\\"Stranger Things\\\"}\",\"Type\":\"Kontecg.Tests.Runtime.Caching.KontecgCacheData_Tests+MyTestClass, Kontecg.Tests\"}";

            var cacheData = KontecgCacheData.Deserialize(json);

            cacheData.ShouldNotBeNull();
        }

        class MyTestClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
