using System.Collections.Generic;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Interfaces
{
    public class User
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserRole Role { get; init; }
        public IEnumerable<UserContact> Contacts { get; init; } = null!;

        public long? OrganizationId { get; init; }
    }
}
