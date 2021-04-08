using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("user")]
    public class User : Entity
    {
        [Column("username")] public string Username { get; init; } = null!;
        [Column("password_hash")] public byte[] PasswordHash { get; init; } = null!;
        [Column("image_id")] public long? ImageId { get; init; }

        [Column("firstname")] public string FirstName { get; init; } = null!;
        [Column("lastname")] public string LastName { get; init; } = null!;
        [Column("patronymic")] public string? Patronymic { get; init; }
        [Column("contacts")] public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();
        [Column("role")] public UserRole Role { get; init; }
        [Column("organization_id")] public long? OrganizationId { get; init; }
    }
}
