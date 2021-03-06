using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string username,
            string password,
            string firstName,
            string lastName,
            string? patronymic,
            IEnumerable<UserContact> contacts,
            UserRole role,
            Organization? organization);

        Task<User?> GetUserByIdOrNullAsync(long userId);
        Task<User?> GetUserByUsernameOrNullAsync(string username);
        Task<IEnumerable<User>> GetAllOrganizationMembersAsync(Organization organization);

        Task<(int Total, IEnumerable<User> Worker)> GetAllUsersWithRoleAsync(UserRole role, DateTimeOffset maxDate,
            int limit, int offset);

        Task UpdateUserAsync(User currentUser, string userFirstName, string userLastName, string? userPatronymic,
            IEnumerable<UserContact> userContacts);
    }
}
