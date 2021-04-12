using System.Threading.Tasks;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUser(string username,
            string password,
            string firstName,
            string lastName,
            string? patronymic,
            UserRole role,
            Organization? customer);
    }

    public class User
    {
    }
}