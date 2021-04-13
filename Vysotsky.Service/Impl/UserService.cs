using System;
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
            UserContact[] contacts, UserRole role)
        {
            var passwordHash = _hasher.Hash(password);
            var id = await _dataConnection.Users.InsertWithInt64IdentityAsync(() => new UserRecord
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
                Id = id
            };
        }

        public Task<User?> GetUserByIdOrNull(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
