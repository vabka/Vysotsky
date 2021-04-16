using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Users
{
    public class PersistedUserDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public IEnumerable<UserContactDto> Contacts { get; init; } = Array.Empty<UserContactDto>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OrganizationId { get; init; }
    }
}
