namespace Vysotsky.Data
{
    public class Room : Entity
    {
        public long? Owner { get; init; }
        public long FloorId { get; init; }
        public RoomStatus RoomStatus { get; init; }
    }
}
