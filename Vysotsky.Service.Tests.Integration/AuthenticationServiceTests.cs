using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Impl;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Vysotsky.Service.Tests.Integration
{
    public class AuthenticationServiceTests : DatabaseTests
    {
        private readonly SecureHasher _hasher;

        public AuthenticationServiceTests()
        {
            _hasher = new SecureHasher(new SecureHasherOptions
            {
                Salt = "0"
            });
            _authenticationService = new AuthenticationService(Database, _hasher, new AuthenticationServiceOptions
            {
                Secret = "secret"
            });
        }

        private readonly AuthenticationService _authenticationService;

        [Fact]
        public async Task SuccessfullyIssueTokenWithCorrectCredentials()
        {
            await CreateAdminAsync();

            var container = await _authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");

            container.Should().NotBeNull();

            var payload = DecodeToken(container!.Token);

            payload["sub"].Should().Be("admin");
            payload["exp"].Should().BeOfType<long>();
            payload["iat"].Should().BeOfType<long>();
            payload.ContainsKey("jti").Should().BeTrue();
        }

        private static Dictionary<string, object> DecodeToken(string? token)
        {
            return JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret("secret")
                .Decode<Dictionary<string, object>>(token);
        }

        private Task<long> CreateAdminAsync() =>
            Database.Users.InsertWithInt64IdentityAsync(() => new UserRecord
            {
                Username = "admin",
                PasswordHash = _hasher.Hash("1234"),
                Role = UserRole.SuperUser,
                OrganizationId = null,
                FirstName = "Иван",
                LastName = "Иванов",
                Patronymic = "Иванович",
                ImageId = null,
                Contacts = Array.Empty<UserContact>(),
                LastPasswordChange = DateTimeOffset.Now
            });

        [Fact]
        public async Task SuccessfullyValidateIssuedToken()
        {
            await CreateAdminAsync();
            var container = await _authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");
            (await _authenticationService.ValidateTokenAsync(container!.Token)).Should().BeTrue();
        }

        [Fact]
        public async Task SuccessfullyRevokeIssuedToken()
        {
            await CreateAdminAsync();

            var container = await _authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");
            await _authenticationService.RevokeTokenAsync(container!.Token);
            (await _authenticationService.ValidateTokenAsync(container!.Token)).Should().BeFalse();
        }
    }
}
