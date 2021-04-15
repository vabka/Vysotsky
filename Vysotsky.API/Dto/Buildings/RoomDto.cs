namespace Vysotsky.API.Dto.Buildings
{
    public class RoomDto
    {
        public RoomDto(string? name)
        {
            Name = name;
        }

        public string Number { get; init; } = "";

        public RoomStatusDto Status { get; init; }

        public string? Name { get; init; }
    }
}
