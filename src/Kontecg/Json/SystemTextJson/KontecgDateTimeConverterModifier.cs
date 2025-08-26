using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Kontecg.Reflection;
using Kontecg.Timing;

namespace Kontecg.Json.SystemTextJson
{
    public class KontecgDateTimeConverterModifier
    {
        private readonly List<string> _inputDateTimeFormats;
        private readonly string _outputDateTimeFormat;

        public KontecgDateTimeConverterModifier(List<string> inputDateTimeFormats, string outputDateTimeFormat)
        {
            _inputDateTimeFormats = inputDateTimeFormats;
            _outputDateTimeFormat = outputDateTimeFormat;
        }

        public Action<JsonTypeInfo> CreateModifyAction()
        {
            return Modify;
        }

        private void Modify(JsonTypeInfo jsonTypeInfo)
        {
            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(jsonTypeInfo.Type) != null)
            {
                return;
            }

            foreach (var property in jsonTypeInfo.Properties.Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?)))
            {
                if (property.AttributeProvider == null ||
                    !property.AttributeProvider.GetCustomAttributes(typeof(DisableDateTimeNormalizationAttribute), false).Any())
                {
                    property.CustomConverter = property.PropertyType == typeof(DateTime)
                        ? (JsonConverter) new KontecgDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat)
                        : new KontecgNullableDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat);
                }
            }
        }
    }
}
