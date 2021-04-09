using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using JWT.Algorithms;
using JWT.Builder;
using LinqToDB;
using LinqToDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Migrations;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Vysotsky.Service.Tests.Integration
{
    public class AuthenticationServiceTests : IDisposable
    {
        private readonly VysotskyDataConnection _database;

        private readonly SecureHasher _hasher;

        public AuthenticationServiceTests()
        {
            var connectionString = "User ID=postgres;Password=postgres;Host=localhost;Port=5433;Database=vysotsky";
            var serviceProvider = Migrator.CreateServices(connectionString);
            using (var scope = serviceProvider.CreateScope())
            {
                Migrator.UpdateDatabase(serviceProvider);
            }

            var options = new LinqToDbConnectionOptionsBuilder()
                .UsePostgreSQL(connectionString)
                .Build<VysotskyDataConnection>();
            _database = new VysotskyDataConnection(options);
            DropDatabase(_database);
            _hasher = new SecureHasher(new byte[] {0});
            _authenticationService = new AuthenticationService(_database, _hasher, "secret");
        }

        private readonly AuthenticationService _authenticationService;

        [Fact]
        public async Task SuccessfullyIssueTokenWithCorrectCredentials()
        {
            var id = await CreateAdminAsync();

            var container = await _authenticationService.TryIssueTokenByUserCredentialsAsync("admin", "1234");

            container.Should().NotBeNull();

            var payload = DecodeToken(container.Token);

            payload["role"].Should().Be("SuperUser");
            payload["user_id"].Should().Be(id);
            payload["name"].Should().Be("admin");
            payload["exp"].Should().BeOfType<long>();
            payload["iat"].Should().BeOfType<long>();
        }

        private static Dictionary<string, object> DecodeToken(string? token)
        {
            return JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret("secret")
                .Decode<Dictionary<string, object>>(token);
        }

        private Task<long> CreateAdminAsync() =>
            _database.Users.InsertWithInt64IdentityAsync(() => new User
            {
                Username = "admin",
                PasswordHash = _hasher.Hash("1234"),
                Role = UserRole.SuperUser,
                OrganizationId = null,
                FirstName = "Иван",
                LastName = "Иванов",
                Patronymic = "Иванович",
                ImageId = null,
                Contacts = Array.Empty<UserContact>()
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

        public void Dispose()
        {
            DropDatabase(_database);
            _database?.Dispose();
        }

        private static void DropDatabase(VysotskyDataConnection connection)
        {
            connection.BlockedTokens.Delete(_ => true);
            connection.Users.Delete(_ => true);
        }
    }
}