using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kontecg.Extensions;

namespace Kontecg.Json.SystemTextJson
{
    public class KontecgStringToEnumFactory : JsonConverterFactory
    {
        private readonly JsonNamingPolicy _namingPolicy;
        private readonly bool _allowIntegerValues;

        public KontecgStringToEnumFactory()
            : this(namingPolicy: null, allowIntegerValues: true)
        {

        }

        public KontecgStringToEnumFactory(JsonNamingPolicy namingPolicy, bool allowIntegerValues)
        {
            _namingPolicy = namingPolicy;
            _allowIntegerValues = allowIntegerValues;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(KontecgStringToEnumConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                new object[] { _namingPolicy, _allowIntegerValues },
                culture: null);
        }
    }

    public class KontecgStringToEnumConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        private readonly JsonStringEnumConverter _innerJsonStringEnumConverter;

        private JsonSerializerOptions _readJsonSerializerOptions;

        private JsonSerializerOptions _writeJsonSerializerOptions;

        public KontecgStringToEnumConverter()
            : this(namingPolicy: null, allowIntegerValues: true)
        {

        }

        public KontecgStringToEnumConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegerValues = true)
        {
            _innerJsonStringEnumConverter = new JsonStringEnumConverter(namingPolicy, allowIntegerValues);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (_readJsonSerializerOptions == null)
            {
                _readJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, x =>
                        x == this ||
                        x.GetType() == typeof(KontecgStringToEnumFactory),
                    _innerJsonStringEnumConverter.CreateConverter(typeToConvert, options));
            }

            return JsonSerializer.Deserialize<T>(ref reader, _readJsonSerializerOptions);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (_writeJsonSerializerOptions == null)
            {
                _writeJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, x =>
                    x == this ||
                    x.GetType() == typeof(KontecgStringToEnumFactory));
            }

            JsonSerializer.Serialize(writer, value, _writeJsonSerializerOptions);
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.PropertyName)
            {
                var str = reader.GetString();
                if (!str.IsNullOrWhiteSpace())
                {
                    return (T)Enum.Parse(typeToConvert, str);
                }
            }

            return base.ReadAsPropertyName(ref reader, typeToConvert, options);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(Enum.GetName(typeof(T), value));
        }
    }

}
