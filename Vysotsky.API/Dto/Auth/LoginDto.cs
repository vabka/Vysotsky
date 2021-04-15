namespace Vysotsky.API.Dto.Auth
{
    public class LoginDto
    {
        public string Username { get; init; } = null!;

        public string Password { get; init; } = null!;

        public bool LongLiving { get; init; }
    }
}
