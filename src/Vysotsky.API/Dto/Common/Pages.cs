using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Common
{
    /// <summary>
    /// Набор возможных страниц для перехода
    /// </summary>
    public class Pages
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationParameters? Next { get; init; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationParameters? Previous { get; init; }
    }
}
