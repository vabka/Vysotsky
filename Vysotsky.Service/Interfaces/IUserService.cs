using System.Threading.Tasks;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUserAsync(string username,
            string password,
            string firstName,
            string lastName,
            string? patronymic,
            UserContact[] contacts,
            UserRole role);

        public Task<User?> GetUserByIdOrNull(long userId);
        public Task<User?> GetUserByUsernameOrNullAsync(string username);
    }

    public class User
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string Firstname { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserRole Role { get; init; }
        public UserContact[] Contacts { get; init; } = null!;
        public long? OrganizationId { get; init; }
    }
}
