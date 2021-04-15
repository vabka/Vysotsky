namespace Vysotsky.API.Dto.Buildings
{
    public class RoomDto
    {
        public RoomDto(string? name) => this.Name = name;

        public string Number { get; init; } = "";

        public RoomStatusDto Status { get; init; }

        public string? Name { get; init; }
    }
}
