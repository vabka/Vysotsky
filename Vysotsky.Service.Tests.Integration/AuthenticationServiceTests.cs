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
        private readonly SecureHasher hasher;

        public AuthenticationServiceTests()
        {
            this.hasher = new SecureHasher(new SecureHasherOptions
            {
                Salt = "0"
            });
            this.authenticationService = new AuthenticationService(this.____RULE_VIOLATION____Database____RULE_VIOLATION____, this.hasher, new AuthenticationServiceOptions
            {
                Secret = "secret"
            });
        }

        private readonly AuthenticationService authenticationService;

        [Fact]
        public async Task SuccessfullyIssueTokenWithCorrectCredentials()
        {
            await CreateAdminAsync();

            var container = await this.authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");

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
            this.____RULE_VIOLATION____Database____RULE_VIOLATION____.Users.InsertWithInt64IdentityAsync(() => new UserRecord
            {
                Username = "admin",
                PasswordHash = this.hasher.Hash("1234"),
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
            var container = await this.authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");
            (await this.authenticationService.ValidateTokenAsync(container!.Token)).Should().BeTrue();
        }

        [Fact]
        public async Task SuccessfullyRevokeIssuedToken()
        {
            await CreateAdminAsync();

            var container = await this.authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");
            await this.authenticationService.RevokeTokenAsync(container!.Token);
            (await this.authenticationService.ValidateTokenAsync(container!.Token)).Should().BeFalse();
        }
    }
}
