using System.Threading.Tasks;
using LinqToDB.Data;
using Vysotsky.Data;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Impl
{
    public class AtomicService : IAtomicService
    {
        private readonly VysotskyDataConnection _dataConnection;

        public AtomicService(VysotskyDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public async Task<IAtomicOperation> BeginAtomicOperationAsync() =>
            new AtomicOperation(await _dataConnection.BeginTransactionAsync());

        private class AtomicOperation : IAtomicOperation
        {
            private readonly DataConnectionTransaction _transaction;

            public AtomicOperation(DataConnectionTransaction transaction) =>
                _transaction = transaction;

            public async ValueTask DisposeAsync() =>
                await _transaction.DisposeAsync();

            public async Task CompleteAsync() =>
                await _transaction.CommitAsync();
        }
    }
}