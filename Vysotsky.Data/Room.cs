namespace Vysotsky.Data
{
    public class Room : Entity
    {
        public long? Owner { get; init; }
        public int Floor { get; init; }
        public long BuildingId { get; init; }
        public RoomStatus RoomStatus { get; init; }
    }
}