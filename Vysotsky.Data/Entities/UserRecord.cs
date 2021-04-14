using System;
using LinqToDB;
using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("user")]
    public class UserRecord : SortableEntity
    {
        [Column("username")] public string Username { get; init; } = null!;
        [Column("password_hash")] public byte[] PasswordHash { get; init; } = null!;
        [Column("image_id")] public long? ImageId { get; init; }

        [Column("firstname")] public string FirstName { get; init; } = null!;
        [Column("lastname")] public string LastName { get; init; } = null!;
        [Column("last_password_change")] public DateTimeOffset LastPasswordChange { get; init; }
        [Column("patronymic")] public string? Patronymic { get; init; }

        [Column("contacts", DataType = DataType.BinaryJson)]
        public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();

        [Column("role", DbType = "user_role")] public UserRole Role { get; init; }
        [Column("organization_id")] public long? OrganizationId { get; init; }
    }
}