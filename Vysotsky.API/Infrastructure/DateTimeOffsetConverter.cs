using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Infrastructure
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options) =>
            reader.TryGetDateTimeOffset(out var value)
                ? value
                : throw new InvalidOperationException("Cannot parse DateTimeOffset");

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToUniversalTime());
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TryGetDateTime(out var value)
                ? value
                : throw new InvalidOperationException("Cannot parse DateTime");

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToUniversalTime());
    }
}
