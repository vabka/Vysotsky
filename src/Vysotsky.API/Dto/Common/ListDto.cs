using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Common
{
    public class ListDto<T>
    {
        [JsonPropertyName("data")] public IEnumerable<T> Data { get; init; } = Array.Empty<T>();
    }
}
