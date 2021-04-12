using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Vysotsky.Data;
using Vysotsky.Data.Entities;

namespace Vysotsky.API.Infrastructure
{
    public class RevokableJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private readonly VysotskyDataConnection _db;

        public RevokableJwtSecurityTokenHandler(IServiceProvider serviceProvider)
        {
            _db = serviceProvider.GetRequiredService<VysotskyDataConnection>();
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            if (validatedToken is JwtSecurityToken
                {Id: var id, IssuedAt: var issuedAt, ValidTo: var validTo, Subject: var username})
            {
                if (Guid.TryParse(id, out var guid) && JtiBlocked(guid))
                {
                    throw LogHelper.LogExceptionMessage(new SecurityTokenRevokedException(
                        LogHelper.FormatInvariant("The token has been revoked, securitytoken: '{0}'.",
                            validatedToken)));
                }

                if (GetLastPasswordChange(username) >= issuedAt)
                {
                    if (Guid.TryParse(id, out guid))
                        BlockTokenByJti(guid, validTo);
                    throw LogHelper.LogExceptionMessage(new SecurityTokenIsNotActualException(
                        LogHelper.FormatInvariant(
                            "The token issued before last password change, securitytoken: '{0}'.",
                            validatedToken)));
                }
            }

            return claimsPrincipal;
        }

        private DateTimeOffset? GetLastPasswordChange(string username)
        {
            var user = _db.Users
                .Where(x => x.Username == username)
                .Select(x => new {x.LastPasswordChange})
                .SingleOrDefault();
            return user?.LastPasswordChange;
        }

        private static (Claim? jti, Claim? iat, Claim? exp) GetClaims(ClaimsPrincipal claimsPrincipal)
        {
            return (claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti),
                claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Iat),
                claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Exp));
        }

        private bool JtiBlocked(Guid jtiGuid) => _db.BlockedTokens.Any(x => x.Jti == jtiGuid);

        private void BlockTokenByJti(Guid jti, DateTimeOffset expiresAt)
        {
            _db.BlockedTokens.Insert(() => new BlockedTokenRecord
            {
                Jti = jti,
                ExpirationTime = expiresAt
            });
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