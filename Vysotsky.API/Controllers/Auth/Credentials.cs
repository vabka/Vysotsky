namespace Vysotsky.API.Controllers.Auth
{
    public class Credentials
    {
        public string Username { get; init; } = null!;

        public string Password { get; init; } = null!;

        public bool LongLiving { get; init; }
    }
}