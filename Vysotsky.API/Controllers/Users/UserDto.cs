using System;

namespace Vysotsky.API.Controllers.Users
{
    public class UserCredentials
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class PersonName
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
    }

    public class UserContact
    {
    }

    public class UserDto
    {
        public UserCredentials Credentials { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
        public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();
        public UserRoleDto RoleDto { get; init; }

        public string? Image { get; init; }
        //public CustomerProps? Customer { get; init; }
    }

    public enum UserRoleDto
    {
        Supervisor,
        Worker,
        Customer,
        CustomerRepresentative
    }
}