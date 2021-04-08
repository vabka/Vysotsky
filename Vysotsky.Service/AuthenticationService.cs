using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service
{
    public interface IAuthenticationService
    {
        Task<string?> TryIssueTokenByUserCredentials(string username, string password, bool longLiving = false);
        Task RevokeToken(string token);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly Database _database;
        private readonly SecureHasher _hasher;
        private readonly string _secret;

        public AuthenticationService(Database database, SecureHasher hasher, string secret)
        {
            _database = database;
            _hasher = hasher;
            _secret = secret;
        }

        public async Task<string?> TryIssueTokenByUserCredentials(string username, string password,
            bool longLiving = false)
        {
            var now = DateTimeOffset.UtcNow;
            var passwordHash = _hasher.Hash(password);
            var user = await _database.Users
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
            return user?.PasswordHash.Equals(passwordHash) == true
                ? JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(_secret)
                    .WithSerializer(new SystemTextJsonSerializer())
                    .AddClaim("exp", now.AddDays(longLiving ? 180 : 1).ToUnixTimeSeconds())
                    .AddClaim("iat", now.ToUnixTimeSeconds())
                    .AddClaim("role", user.Role.ToString())
                    .AddClaim("user_id", user.Id)
                    .AddClaim("username", user.Username)
                    .AddClaim("organization_id", user.OrganizationId)
                    .Encode()
                : null;
        }

        public async Task RevokeToken(string token)
        {
            var payload = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .WithSerializer(new SystemTextJsonSerializer())
                .Decode<TokenPayload>(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(payload.Expiration);
            if (expirationTime < DateTimeOffset.Now)
                await _database.BlockedTokens.InsertAsync(() => new BlockedToken
                {
                    UserId = payload.UserId,
                    ExpirationTime = expirationTime,
                    Value = token
                });
        }

        private class SystemTextJsonSerializer : IJsonSerializer
        {
            private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.Strict,
                WriteIndented = false,
                AllowTrailingCommas = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            public string Serialize(object obj) =>
                JsonSerializer.Serialize(obj, Options);

            public T? Deserialize<T>(string json) =>
                JsonSerializer.Deserialize<T>(json, Options);
        }

        private record TokenPayload
        {
            [JsonPropertyName("user_id")] public long UserId { get; init; }
            [JsonPropertyName("username")] public string Username { get; init; } = null!;
            [JsonPropertyName("organization_id")] public long? OrganizationId { get; init; }
            [JsonPropertyName("role")] public UserRole Role { get; init; }
            [JsonPropertyName("exp")] public int Expiration { get; init; }
        }
    }
}
