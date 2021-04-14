using System;
using Vysotsky.API.Dto.Organizations;

namespace Vysotsky.API.Dto.Users
{
    public class UserDto
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
        public UserContactDto[] Contacts { get; init; } = Array.Empty<UserContactDto>();
        public UserRoleDto Role { get; init; }

        public string? Image { get; init; }
        public OrganizationDto? Organization { get; init; }
    }
}
