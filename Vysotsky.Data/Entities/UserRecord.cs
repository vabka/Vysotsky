using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("user")]
    public class UserRecord : Entity
    {
        [Column("username")] public string Username { get; init; } = null!;
        [Column("password_hash")] public byte[] PasswordHash { get; init; } = null!;
        [Column("image_id")] public long? ImageId { get; init; }

        [Column("firstname")] public string FirstName { get; init; } = null!;
        [Column("lastname")] public string LastName { get; init; } = null!;
        [Column("last_password_change")] public DateTimeOffset LastPasswordChange { get; init; }
        [Column("patronymic")] public string? Patronymic { get; init; }

        [Column("contacts", DataType = DataType.BinaryJson)]
        public UserContact[] Contacts { get; init; } = null!;

        [Column("role")] public UserRole Role { get; init; }
        [Column("organization_id")] public long? OrganizationId { get; init; }
    }

    public enum UserRole
    {
        [MapValue("SuperUser")] SuperUser,
        [MapValue("Supervisor")] Supervisor,
        [MapValue("Worker")] Worker,
        [MapValue("OrganizationOwner")] OrganizationOwner,
        [MapValue("OrganizationMember")] OrganizationMember
    }

    public record UserContact
    {
        public string Name { get; init; } = null!;
        public string Value { get; init; } = null!;
        public ContactType Type { get; init; }
    }

    public enum ContactType
    {
        Phone,
        Telegram,
        Whatsapp,
    }
}
