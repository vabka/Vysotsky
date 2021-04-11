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
        private readonly VysotskyDataConnection _vysotskyDataConnection;
        private readonly IStringHasher _hasher;
        private readonly AuthenticationServiceOptions _options;

        public AuthenticationService(VysotskyDataConnection vysotskyDataConnection, IStringHasher hasher,
            AuthenticationServiceOptions options)
        {
            _vysotskyDataConnection = vysotskyDataConnection;
            _hasher = hasher;
            _options = options;
        }

        public async Task<TokenContainer?> TryIssueTokenByUserCredentialsAsync(string username, string password,
            bool longLiving = false)
        {
            var now = DateTimeOffset.UtcNow;
            var passwordHash = _hasher.Hash(password);
            var user = await _vysotskyDataConnection.Users
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
            var exp = now.AddDays(longLiving ? 180 : 1);
            var iat = now;
            if (user?.PasswordHash.SequenceEqual(passwordHash) == true)
            {
                var token = EncodeToken(new TokenPayload
                {
                    Sub = user.Username,
                    Role = user.Role,
                    Exp = exp.ToUnixTimeSeconds(),
                    Iat = iat.ToUnixTimeSeconds()
                });
                return new TokenContainer(token, exp, iat);
            }

            return null;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var payload = DecodeToken(token);
            return await _vysotskyDataConnection.BlockedTokens.AllAsync(x => x.Jti != payload.Jti);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var payload = DecodeToken(token);
            var exp = DateTimeOffset.FromUnixTimeSeconds(payload.Exp);
            await _vysotskyDataConnection.BlockedTokens.InsertAsync(() => new BlockedTokenRecord
            {
                ExpirationTime = exp,
                Jti = payload.Jti
            });
        }

        private string EncodeToken(TokenPayload payload) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .AddClaim("exp", payload.Exp)
                .AddClaim("iat", payload.Iat)
                .AddClaim("role", payload.Role.ToString())
                .AddClaim("sub", payload.Sub)
                .AddClaim("jti", Guid.NewGuid())
                .Encode();

        private TokenPayload DecodeToken(string token) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .MustVerifySignature()
                .Decode<TokenPayload>(token);


        private record TokenPayload
        {
            [JsonProperty("jti")] public Guid Jti { get; init; }
            [JsonProperty("sub")] public string Sub { get; init; } = null!;
            [JsonProperty("role")] public UserRole Role { get; init; }
            [JsonProperty("exp")] public long Exp { get; init; }
            [JsonProperty("iat")] public long Iat { get; init; }
        }
    }

    public class AuthenticationServiceOptions
    {
        public string Secret { get; init; }
    }
}
