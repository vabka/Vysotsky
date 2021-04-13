using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly VysotskyDataConnection _dataConnection;
        private readonly IStringHasher _hasher;

        public UserService(VysotskyDataConnection dataConnection, IStringHasher hasher)
        {
            _dataConnection = dataConnection;
            _hasher = hasher;
        }

        public async Task<User> RegisterUserAsync(string username, string password, string firstName, string lastName,
            string? patronymic,
            UserContact[] contacts,
            UserRole role)
        {
            var passwordHash = _hasher.Hash(password);
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
                });
            return new User
            {
                Id = id,
                Firstname = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                Username = username,
                Contacts = contacts,
                Role = role,
                OrganizationId = null
            };
        }

        public Task<User?> GetUserByIdOrNull(long userId) =>
            _dataConnection.Users
                .Where(u => u.Id == userId)
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Firstname = u.FirstName,
                    LastName = u.LastName,
                    Patronymic = u.Patronymic,
                    Role = u.Role,
                    Contacts = u.Contacts,
                    OrganizationId = u.OrganizationId
                })
                .SingleOrDefaultAsync();

        public Task<User?> GetUserByUsernameOrNullAsync(string username) =>
            _dataConnection.Users
                .Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Firstname = u.FirstName,
                    LastName = u.LastName,
                    Patronymic = u.Patronymic,
                    Role = u.Role,
                    Contacts = u.Contacts,
                    OrganizationId = u.OrganizationId
                })
                .SingleOrDefaultAsync();
    }
}
