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
    }

    public class User
    {

        public long Id { get; init; }
        public string Username { get; init; } = null!;
    }
}