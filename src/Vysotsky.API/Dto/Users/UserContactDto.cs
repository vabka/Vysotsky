namespace Vysotsky.API.Dto.Users
{
    public class UserContactDto
    {
        public UserContactTypeDto Type { get; init; } = UserContactTypeDto.Phone;
        public string Name { get; init; } = null!;
        public string Value { get; init; } = null!;
    }
}
