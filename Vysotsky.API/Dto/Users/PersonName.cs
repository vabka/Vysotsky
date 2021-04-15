namespace Vysotsky.API.Dto.Users
{
    public class PersonName
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
    }
}
