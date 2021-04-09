using System;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Vysotsky.Data;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service
{
    public interface IAuthenticationService
    {
        Task<TokenContainer?> TryIssueTokenByUserCredentialsAsync(string username, string password,
            bool longLiving = false);

        Task RevokeTokenAsync(string token);
    }

    public record TokenContainer(string Token, DateTimeOffset ExpiresAt, DateTimeOffset IssuedAt);

    public class AuthenticationService : IAuthenticationService
    {
        private readonly VysotskyDataConnection _vysotskyDataConnection;
        private readonly IStringHasher _hasher;
        private readonly string _secret;

        public AuthenticationService(VysotskyDataConnection vysotskyDataConnection, IStringHasher hasher, string secret)
        {
            _vysotskyDataConnection = vysotskyDataConnection;
            _hasher = hasher;
            _secret = secret;
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
                .AddClaim("name", payload.Username)
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
            [JsonProperty("name")] public string Username { get; init; } = null!;
            [JsonProperty("role")] public UserRole Role { get; init; }
            [JsonProperty("exp")] public long Expiration { get; init; }
            [JsonProperty("iat")] public long IssuedAt { get; init; }
        }
    }

    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IAuthenticationService>(s =>
            {
                var secret = s.GetRequiredService<IConfiguration>()["SECRET"];
                return new AuthenticationService(
                    s.GetRequiredService<VysotskyDataConnection>(),
                    s.GetRequiredService<IStringHasher>(),
                    secret
                );
            });
        }
    }
}