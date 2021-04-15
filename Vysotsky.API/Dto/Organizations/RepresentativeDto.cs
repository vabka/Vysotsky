using Vysotsky.API.Dto.Users;

namespace Vysotsky.API.Dto.Organizations
{
    public class RepresentativeDto
    {
        public long Id { get; set; }
        public string Username { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
    }
}
