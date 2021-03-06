using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class Room
    {
        public long Id { get; init; }
        public string? Number { get; init; }
        public RoomStatus Status { get; init; }
        public long? OwnerId { get; set; }
    }
}
