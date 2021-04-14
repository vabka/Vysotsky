using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> CreateUserAsync(string username,
            string password,
            string firstName,
            string lastName,
            string? patronymic,
            IEnumerable<UserContact> contacts,
            UserRole role,
            Organization? organization);

        public Task<User?> GetUserByIdOrNull(long userId);
        public Task<User?> GetUserByUsernameOrNullAsync(string username);
        public Task<IEnumerable<User>> GetAllOrganizationMembersAsync(Organization organization);
    }
}