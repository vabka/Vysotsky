using Microsoft.IdentityModel.Tokens;

namespace Vysotsky.API.Infrastructure
{
    public class SecurityTokenInvalidException : SecurityTokenException
    {
        public SecurityTokenInvalidException(string message) : base(message)
        {
        }

        public SecurityTokenInvalidException()
        {
        }

        public SecurityTokenInvalidException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
