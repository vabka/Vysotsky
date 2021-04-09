using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class Room
    {
        public long Id { get; set; }
        public string? Number { get; set; }
        public string? Name { get; set; }
        public RoomStatus Status { get; set; }
    }
}