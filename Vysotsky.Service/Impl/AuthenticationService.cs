using System;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using Newtonsoft.Json;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly VysotskyDataConnection vysotskyDataConnection;
        private readonly IStringHasher hasher;
        private readonly AuthenticationServiceOptions options;

        public AuthenticationService(VysotskyDataConnection vysotskyDataConnection, IStringHasher hasher,
            AuthenticationServiceOptions options)
        {
            this.vysotskyDataConnection = vysotskyDataConnection;
            this.hasher = hasher;
            this.options = options;
        }

        public async Task<TokenContainer?> TryIssueTokenByUserCredentialsAsync(string username, string password,
            bool longLiving = false)
        {
            var now = DateTimeOffset.UtcNow;
            var user = await this.vysotskyDataConnection.Users
                .Where(x => x.Username == username)
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.PasswordHash,
                    x.Role,
                    x.OrganizationId
                })
                .SingleOrDefaultAsync();
            if (user != null)
            {
                var passwordHash = this.hasher.Hash(password);
                if (user.PasswordHash.SequenceEqual(passwordHash))
                {
                    var exp = now.AddDays(longLiving ? 180 : 1);
                    var iat = now;
                    var token = this.EncodeToken(new TokenPayload
                    {
                        Sub = user.Username,
                        Exp = exp.ToUnixTimeSeconds(),
                        Iat = iat.ToUnixTimeSeconds()
                    });
                    return new TokenContainer(token, exp, iat);
                }
            }


            return null;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var payload = this.DecodeToken(token);
            return await this.vysotskyDataConnection.BlockedTokens.AllAsync(x => x.Jti != payload.Jti);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var payload = this.DecodeToken(token);
            var exp = DateTimeOffset.FromUnixTimeSeconds(payload.Exp);
            await this.RevokeTokenByJtiAsync(payload.Jti, exp);
        }

        public Task RevokeTokenByJtiAsync(Guid tokenIdentifier, DateTimeOffset expiration) =>
            this.vysotskyDataConnection.BlockedTokens
                .InsertAsync(() => new BlockedTokenRecord
                {
                    ExpirationTime = expiration,
                    Jti = tokenIdentifier
                });

        public Task<DateTimeOffset?> TryGetLastPasswordChangeTimeAsync(string username) =>
            this.vysotskyDataConnection.Users
                .Where(u => u.Username == username)
                .Select(u => u.LastPasswordChange)
                .Cast<DateTimeOffset?>()
                .SingleOrDefaultAsync();

        public Task<bool> CheckTokenRevokedAsync(Guid tokenIdentifier) =>
            this.vysotskyDataConnection.BlockedTokens.AnyAsync(x => x.Jti == tokenIdentifier);

        private string EncodeToken(TokenPayload payload) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(this.options.Secret)
                .AddClaim("exp", payload.Exp)
                .AddClaim("iat", payload.Iat)
                .AddClaim("sub", payload.Sub)
                .AddClaim("jti", Guid.NewGuid())
                .Encode();

        private TokenPayload DecodeToken(string token) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(this.options.Secret)
                .MustVerifySignature()
                .Decode<TokenPayload>(token);


        private record TokenPayload
        {
            [JsonProperty("jti")] public Guid Jti { get; init; }
            [JsonProperty("sub")] public string Sub { get; init; } = null!;
            [JsonProperty("exp")] public long Exp { get; init; }
            [JsonProperty("iat")] public long Iat { get; init; }
        }
    }
}
