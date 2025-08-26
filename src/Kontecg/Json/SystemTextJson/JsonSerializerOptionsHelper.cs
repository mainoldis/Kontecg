using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kontecg.Collections.Extensions;

namespace Kontecg.Json.SystemTextJson
{
    public static class JsonSerializerOptionsHelper
    {
        public static JsonSerializerOptions Create(JsonSerializerOptions baseOptions, JsonConverter removeConverter, params JsonConverter[] addConverters)
        {
            return Create(baseOptions, x => x == removeConverter, addConverters);
        }

        public static JsonSerializerOptions Create(JsonSerializerOptions baseOptions, Func<JsonConverter, bool> removeConverterPredicate, params JsonConverter[] addConverters)
        {
            var options = new JsonSerializerOptions(baseOptions);

            var items = options.Converters.Where(removeConverterPredicate).ToList();
            foreach (var item in items)
            {
                options.Converters.Remove(item);
            }

            foreach (var jsonConverter in addConverters)
            {
                options.Converters.AddIfNotContains(jsonConverter);
            }
            return options;
        }
    }

}
