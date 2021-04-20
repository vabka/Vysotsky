namespace Vysotsky.API.Dto.Chats
{
    public class ShortPersistedUserDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public long? OrganizationId { get; init; }
    }
}