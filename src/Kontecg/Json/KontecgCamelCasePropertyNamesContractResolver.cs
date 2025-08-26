using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontecg.Json
{
    internal class KontecgCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        public KontecgCamelCasePropertyNamesContractResolver()
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
