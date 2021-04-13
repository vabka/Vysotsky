using System;

namespace Vysotsky.API.Controllers.Users
{
    public class PersonName
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
    }

    public class UserContactDto
    {
        public UserContactTypeDto Type { get; init; } = UserContactTypeDto.Phone;
        public string Name { get; init; } = null!;
        public string Value { get; init; } = null!;
    }

    public enum UserContactTypeDto
    {
        Phone
    }

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

    public enum UserRoleDto
    {
        Supervisor,
        Worker,
        Customer,
        CustomerRepresentative
    }
}
