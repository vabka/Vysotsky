using System.Text.Json.Serialization;

namespace Vysotsky.API.Models
{
    /// <summary>
    /// Набор возможных страниц для перехода
    /// </summary>
    public class Pages
    {
        /// <summary>
        /// URL Предыдущей страницы
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Previous { get; init; }

        /// <summary>
        /// URL следующей страницы
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Next { get; init; }
    }
}