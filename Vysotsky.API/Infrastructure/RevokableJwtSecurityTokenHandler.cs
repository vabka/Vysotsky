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
            out SecurityToken validatedToken)
        {
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            return validatedToken switch
            {
                JwtSecurityToken { Id: var id, IssuedAt: var issuedAt, ValidTo: var validTo, Subject: not "" }
                    when Guid.TryParse(id, out _) &&
                         issuedAt != default &&
                         validTo != default
                    => claimsPrincipal,
                _ => throw LogHelper.LogExceptionMessage(new SecurityTokenInvalidException(
                    LogHelper.FormatInvariant("Invalid securitytoken: '{0}'.",
                        validatedToken)))
            };
        }
    }
}
