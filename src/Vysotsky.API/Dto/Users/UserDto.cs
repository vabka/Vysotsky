using System;
using Vysotsky.API.Dto.Organizations;

namespace Vysotsky.API.Dto.Users
{
    public class UserDto
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;

        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserContactDto[] Contacts { get; init; } = Array.Empty<UserContactDto>();
        public UserRoleDto Role { get; init; }

        public OrganizationDto? Organization { get; init; }
    }
}
