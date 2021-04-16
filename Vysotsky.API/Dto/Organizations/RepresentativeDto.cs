
namespace Vysotsky.API.Dto.Organizations
{
    public class RepresentativeDto
    {
        public long Id { get; set; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
    }
}
