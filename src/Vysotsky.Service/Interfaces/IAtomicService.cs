using System.Threading.Tasks;

namespace Vysotsky.Service.Interfaces
{
    public interface IAtomicService
    {
        public Task<IAtomicOperation> BeginAtomicOperationAsync();
    }
}
