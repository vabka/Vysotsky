using System.Threading.Tasks;

namespace Vysotsky.Service.Interfaces
{
    public interface IWorkerService
    {
        Task<User?> GetWorkerByIdOrNullAsync(long workerId);
    }
}