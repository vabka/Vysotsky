namespace Vysotsky.API.Dto.Buildings
{
    public class RoomDto
    {
        public string Number { get; init; } = "";
        public RoomStatusDto Status { get; init; }
        public long? OwnerId { get; init; }
    }
}
