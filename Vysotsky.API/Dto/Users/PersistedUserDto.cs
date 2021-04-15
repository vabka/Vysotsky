using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Users
{
    public class PersistedUserDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
        public IEnumerable<UserContactDto> Contacts { get; init; } = Array.Empty<UserContactDto>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OrganizationId { get; init; }
    }
}
