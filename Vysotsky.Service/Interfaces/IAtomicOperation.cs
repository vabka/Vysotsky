using System;
using System.Threading.Tasks;

namespace Vysotsky.Service.Interfaces
{
    public interface IAtomicOperation : IAsyncDisposable
    {
        public Task CompleteAsync();
    }
}