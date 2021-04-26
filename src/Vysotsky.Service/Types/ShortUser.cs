using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class ShortUser
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserRole Role { get; init; }
        public long? OrganizationId { get; init; }
    }
}