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
                    UserId = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    Expiration = exp.ToUnixTimeSeconds(),
                    IssuedAt = iat.ToUnixTimeSeconds()
                });
                return new TokenContainer(token, exp, iat);
            }

            return null;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var payload = DecodeToken(token);
            return payload.Expiration > now &&
                   await _vysotskyDataConnection.BlockedTokens.AllAsync(x => x.Value != token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var payload = DecodeToken(token);
            var exp = DateTimeOffset.FromUnixTimeSeconds(payload.Expiration);
            await _vysotskyDataConnection.BlockedTokens.InsertAsync(() => new BlockedTokenRecord
            {
                UserId = payload.UserId,
                ExpirationTime = exp,
                Value = token
            });
        }

        private string EncodeToken(TokenPayload payload)
        {
            return JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .AddClaim("exp", payload.Expiration)
                .AddClaim("iat", payload.IssuedAt)
                .AddClaim("role", payload.Role.ToString())
                .AddClaim("user_id", payload.UserId)
                .AddClaim("name", payload.Username)
                .Encode();
        }

        private TokenPayload DecodeToken(string token) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_options.Secret)
                .MustVerifySignature()
                .Decode<TokenPayload>(token);


        private record TokenPayload
        {
            [JsonProperty("user_id")] public long UserId { get; init; }
            [JsonProperty("name")] public string Username { get; init; } = null!;
            [JsonProperty("role")] public UserRole Role { get; init; }
            [JsonProperty("exp")] public long Expiration { get; init; }
            [JsonProperty("iat")] public long IssuedAt { get; init; }
        }
    }

    public class AuthenticationServiceOptions
    {
        public string Secret { get; set; }
    }
}