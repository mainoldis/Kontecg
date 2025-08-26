using System;
using System.Reflection;
using Kontecg.Configuration.Startup;
using Kontecg.Extensions;
using Kontecg.Reflection;
using Kontecg.Threading.Extensions;
using Kontecg.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontecg.Json
{
    internal class KontecgContractResolver : DefaultContractResolver
    {
        public KontecgContractResolver()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            if (KontecgDateTimeConverter.ShouldNormalize(member, property))
            {
                property.Converter = new KontecgDateTimeConverter();
            }
        }
    }
}
