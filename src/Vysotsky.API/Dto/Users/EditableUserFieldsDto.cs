using System;

namespace Vysotsky.API.Dto.Users
{
    public class EditableUserFieldsDto
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserContactDto[] Contacts { get; init; } = Array.Empty<UserContactDto>();
    }
}
