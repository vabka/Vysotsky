using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Buildings
{
    public class PersistedRoomDto
    {
        public long Id { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Number { get; init; }

        public RoomStatusDto Status { get; set; }
    }
}