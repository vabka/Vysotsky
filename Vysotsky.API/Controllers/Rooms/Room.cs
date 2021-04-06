namespace Vysotsky.API.Controllers.Rooms
{
    public record Room(string Id, string Number, int Floor, RoomStatus Status, int? CustomerId);
}
