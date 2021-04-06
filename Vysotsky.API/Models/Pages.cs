using System.Text.Json.Serialization;

namespace Vysotsky.API.Models
{
    public class Pages
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Previous { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Next { get; set; }
    }
}
