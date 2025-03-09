using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace kwangho.restapi.Filters
{
    /// <summary>
    /// json 변환시 DateTimeOffset을 "yyyy'-'MM'-'dd'T'HH':'mm':'ssK" 포멧의 문자열로 변환
    /// </summary>
    public class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTimeOffset));
            return DateTimeOffset.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK"));
        }
    }
}
