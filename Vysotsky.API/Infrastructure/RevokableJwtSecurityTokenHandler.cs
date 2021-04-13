using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Vysotsky.API.Infrastructure
{
    public class RevokableJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
            out SecurityToken securityToken)
        {
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out securityToken);
            return securityToken switch
            {
                JwtSecurityToken {Id: var id, IssuedAt: var issuedAt, ValidTo: var validTo, Subject: not ""}
                    when Guid.TryParse(id, out _) &&
                         issuedAt != default &&
                         validTo != default
                    => claimsPrincipal,
                _ => throw LogHelper.LogExceptionMessage(new SecurityTokenInvalidException(
                    LogHelper.FormatInvariant("Invalid securitytoken: '{0}'.",
                        securityToken)))
            };
        }
    }

    public class SecurityTokenInvalidException : SecurityTokenException
    {
        public SecurityTokenInvalidException(string message) : base(message)
        {
        }
    }

    public class SecurityTokenRevokedException : SecurityTokenException
    {
        public SecurityTokenRevokedException()
        {
        }

        public SecurityTokenRevokedException(string message) : base(message)
        {
        }

        public SecurityTokenRevokedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

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
