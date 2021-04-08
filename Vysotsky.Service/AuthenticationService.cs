using System;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using Newtonsoft.Json;
using Vysotsky.Data;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service
{
    public interface IAuthenticationService
    {
        Task<string?> TryIssueTokenByUserCredentialsAsync(string username, string password, bool longLiving = false);
        Task RevokeTokenAsync(string token);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly VysotskyDataConnection _vysotskyDataConnection;
        private readonly SecureHasher _hasher;
        private readonly string _secret;

        public AuthenticationService(VysotskyDataConnection vysotskyDataConnection, SecureHasher hasher, string secret)
        {
            _vysotskyDataConnection = vysotskyDataConnection;
            _hasher = hasher;
            _secret = secret;
        }

        public async Task<string?> TryIssueTokenByUserCredentialsAsync(string username, string password,
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
            var exp = now.AddDays(longLiving ? 180 : 1).ToUnixTimeSeconds();
            var iat = now.ToUnixTimeSeconds();
            if (user?.PasswordHash.SequenceEqual(passwordHash) == true)
                return EncodeToken(new TokenPayload
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    OrganizationId = user.OrganizationId,
                    Expiration = exp,
                    IssuedAt = iat
                });
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
            await _vysotskyDataConnection.BlockedTokens.InsertAsync(() => new BlockedToken
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
                .WithSecret(_secret)
                .AddClaim("exp", payload.Expiration)
                .AddClaim("iat", payload.IssuedAt)
                .AddClaim("role", payload.Role.ToString())
                .AddClaim("user_id", payload.UserId)
                .AddClaim("username", payload.Username)
                .AddClaim("organization_id", payload.OrganizationId)
                .Encode();
        }

        private TokenPayload DecodeToken(string token) =>
            JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .MustVerifySignature()
                .Decode<TokenPayload>(token);


        private record TokenPayload
        {
            [JsonProperty("user_id")] public long UserId { get; init; }
            [JsonProperty("username")] public string Username { get; init; } = null!;
            [JsonProperty("organization_id")] public long? OrganizationId { get; init; }
            [JsonProperty("role")] public UserRole Role { get; init; }
            [JsonProperty("exp")] public long Expiration { get; init; }
            [JsonProperty("iat")] public long IssuedAt { get; init; }
        }
    }
}
