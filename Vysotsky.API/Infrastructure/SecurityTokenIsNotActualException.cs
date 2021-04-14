using System;
using Microsoft.IdentityModel.Tokens;

namespace Vysotsky.API.Infrastructure
{
    public class SecurityTokenIsNotActualException : SecurityTokenException
    {
        public SecurityTokenIsNotActualException()
        {
        }

        public SecurityTokenIsNotActualException(string message) : base(message)
        {
        }

        public SecurityTokenIsNotActualException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}