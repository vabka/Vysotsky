using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly VysotskyDataConnection _dataConnection;
        private readonly IStringHasher _hasher;

        private static readonly Expression<Func<UserRecord, User>> MapToUser = u => new User
        {
            Id = u.Id,
            Username = u.Username,
            Firstname = u.FirstName,
            LastName = u.LastName,
            Patronymic = u.Patronymic,
            Role = u.Role,
            Contacts = u.Contacts,
            OrganizationId = u.OrganizationId
        };

        public UserService(VysotskyDataConnection dataConnection, IStringHasher hasher)
        {
            _dataConnection = dataConnection;
            _hasher = hasher;
        }

        public async Task<User> CreateUserAsync(string username,
            string password,
            string firstName,
            string lastName,
            string? patronymic,
            IEnumerable<UserContact> contacts,
            UserRole role,
            Organization? organization)
        {
            var passwordHash = _hasher.Hash(password);
            var contactsArray = contacts.ToArray();
            var organizationId = organization?.Id;
            var id = await _dataConnection.Users
                .InsertWithInt64IdentityAsync(() => new UserRecord
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = patronymic,
                    Role = role,
                    LastPasswordChange = DateTimeOffset.Now,
                    ImageId = null,
                    OrganizationId = organizationId,
                    Contacts = contactsArray
                });
            return new User
            {
                Id = id,
                Firstname = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                Username = username,
                Contacts = contactsArray,
                Role = role,
                OrganizationId = organizationId
            };
        }

        public async Task<User?> GetUserByIdOrNull(long userId) =>
            await _dataConnection.Users
                .Where(u => u.Id == userId)
                .Select(MapToUser)
                .SingleOrDefaultAsync();

        public async Task<User?> GetUserByUsernameOrNullAsync(string username) =>
            await _dataConnection.Users
                .Where(u => u.Username == username)
                .Select(MapToUser)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<User>> GetAllOrganizationMembersAsync(Organization organization) =>
            await _dataConnection.Users
                .Where(u => u.OrganizationId == organization.Id && u.Role == UserRole.OrganizationMember)
                .Select(MapToUser)
                .ToArrayAsync();

        public async Task<IEnumerable<User>> GetAllUsersWithRoleAsync(UserRole role) =>
            await _dataConnection.Users
                .Where(u => u.Role == role)
                .Select(MapToUser)
                .ToArrayAsync();
    }
}