using System;
using System.Threading.Tasks;
using Vysotsky.API.Controllers.Customers;

namespace Vysotsky.API.Controllers.Users
{
    public interface IUserRepository
    {
        Task<IAsyncDisposable> BeginRegistrationProcedure();
        User CreateUser(string username, string password);
        void AttachCustomer(User user, Customer customer);
        void AttachEmployee(User user, Employee employee);
        void AttachSupervisor(User user, Supervisor supervisor);
        Task<bool> IsUniqueUsername(string authUsername);
    }
}