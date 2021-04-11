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
            // make sure everything is valid first to avoid unnecessary calls to DB
            // if it's not valid base.ValidateToken will throw an exception, we don't need to handle it because it's handled here: https://github.com/aspnet/Security/blob/beaa2b443d46ef8adaf5c2a89eb475e1893037c2/src/Microsoft.AspNetCore.Authentication.JwtBearer/JwtBearerHandler.cs#L107-L128
            // we have to throw our own exception if the token is revoked, it will cause validation to fail
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);

            var jtiClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti);
            var subClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);
            var iatClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Iat);
            var expClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Exp);
            if (jtiClaim is {ValueType: ClaimValueTypes.String})
            {
                var claimGuid = Guid.Parse(jtiClaim.Value);
                if (_db.BlockedTokens.Any(x => x.Jti == claimGuid)) // it's blacklisted! throw the exception
                {
                    // there's a bunch of built-in token validation codes: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/7692d12e49a947f68a44cd3abc040d0c241376e6/src/Microsoft.IdentityModel.Tokens/LogMessages.cs
                    // but none of them is suitable for this
                    throw LogHelper.LogExceptionMessage(new SecurityTokenRevokedException(
                        LogHelper.FormatInvariant("The token has been revoked, securitytoken: '{0}'.",
                            validatedToken)));
                }
            }

            if (subClaim is {ValueType: ClaimValueTypes.String, Value: var username} &&
                iatClaim is {ValueType: ClaimValueTypes.Integer64, Value: var iatStr})
            {
                var iat = long.Parse(iatStr);
                var issueTime = DateTimeOffset.FromUnixTimeSeconds(iat);
                var user = _db.Users
                    .Where(x => x.Username == username)
                    .Select(x => new {x.LastPasswordChange})
                    .SingleOrDefault();
                if (user is not {LastPasswordChange: var lastPasswordChange} || lastPasswordChange > issueTime)
                {
                    if (expClaim is {ValueType: ClaimValueTypes.Integer64, Value: var expStr} &&
                        jtiClaim is {ValueType: ClaimValueTypes.String})
                    {
                        var jti = Guid.Parse(jtiClaim.Value);
                        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expStr));
                        _db.BlockedTokens.Insert(() => new BlockedTokenRecord
                        {
                            Jti = jti,
                            ExpirationTime = expiresAt
                        });
                    }
                    
                    throw LogHelper.LogExceptionMessage(new SecurityTokenIsNotActualException(
                        LogHelper.FormatInvariant(
                            "The token issued before last password change, securitytoken: '{0}'.",
                            validatedToken)));
                }
            }


            return claimsPrincipal;
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
