namespace Vysotsky.API.Controllers.Rooms
{
    public class RoomProperties
    {
        public string Number { get; init; } = "";
        public int Floor { get; init; }
        public RoomStatus Status { get; init; }
    }
}