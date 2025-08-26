using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace Kontecg.Json.SystemTextJson
{
    public class KontecgDateTimeJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
    {
        public KontecgDateTimeJsonTypeInfoResolver(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            Modifiers.Add(new KontecgDateTimeConverterModifier(inputDateTimeFormats, outputDateTimeFormat).CreateModifyAction());
        }
    }
}
