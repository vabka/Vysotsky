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
    public class UserService : IUserService, IWorkerService
    {
        private readonly VysotskyDataConnection db;
        private readonly IStringHasher hasher;

        private static readonly Expression<Func<UserRecord, User>> MapToUser = u => new User
        {
            Id = u.Id,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Patronymic = u.Patronymic,
            Role = u.Role,
            Contacts = u.Contacts,
            OrganizationId = u.OrganizationId
        };

        public UserService(VysotskyDataConnection db, IStringHasher hasher)
        {
            this.db = db;
            this.hasher = hasher;
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
            var passwordHash = hasher.Hash(password);
            var contactsArray = contacts.ToArray();
            var organizationId = organization?.Id;
            var id = await db.Users
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
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                Username = username,
                Contacts = contactsArray,
                Role = role,
                OrganizationId = organizationId
            };
        }

        public async Task<User?> GetUserByIdOrNullAsync(long userId) =>
            await db.Users
                .Where(u => u.Id == userId)
                .Select(MapToUser)
                .FirstOrDefaultAsync();

        public async Task<User?> GetUserByUsernameOrNullAsync(string username) =>
            await db.Users
                .Where(u => u.Username == username)
                .Select(MapToUser)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<User>> GetAllOrganizationMembersAsync(Organization organization) =>
            await db.Users
                .Where(u => u.OrganizationId == organization.Id && u.Role == UserRole.OrganizationMember)
                .Select(MapToUser)
                .ToArrayAsync();


        public async Task<(int Total, IEnumerable<User> Worker)> GetAllUsersWithRoleAsync(UserRole role,
            DateTimeOffset maxDate, int limit,
            int offset)
        {
            var query = db.Users
                .Where(u => u.CreatedAt < maxDate)
                .Where(u => u.Role == role)
                .Select(MapToUser);
            var total = await query.CountAsync();
            var workers = await query
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
            return (total, workers);
        }

        public Task UpdateUserAsync(User currentUser, string userFirstName, string userLastName, string? userPatronymic,
            IEnumerable<UserContact> userContacts) =>
            throw new NotImplementedException();

        public async Task<IEnumerable<User>> GetAllUsersWithRoleAsync(UserRole role) =>
            await db.Users
                .Where(u => u.Role == role)
                .Select(MapToUser)
                .ToArrayAsync();

        public async Task<User?> GetWorkerByIdOrNullAsync(long workerId) =>
            await db.Users.Where(u => u.Role == UserRole.Worker)
                .Where(u => u.Id == workerId)
                .Select(MapToUser)
                .FirstOrDefaultAsync();
    }
}
