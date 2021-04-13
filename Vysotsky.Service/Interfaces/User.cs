using System.Text.Json.Serialization;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Interfaces
{
    public class User
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string Firstname { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserRole Role { get; init; }
        public UserContact[] Contacts { get; init; } = null!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OrganizationId { get; init; }
    }
}
