using System;

namespace Vysotsky.Data
{
    public class User : Entity
    {
        public string Username { get; init; } = null!;
        public byte[] PasswordHash { get; init; } = null!;
        public long? ImageId { get; init; }

        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();
        public UserRole Role { get; init; }
    }
}