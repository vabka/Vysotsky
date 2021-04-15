using System.Threading.Tasks;
using LinqToDB.Data;
using Vysotsky.Data;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Impl
{
    public class AtomicService : IAtomicService
    {
        private readonly VysotskyDataConnection dataConnection;

        public AtomicService(VysotskyDataConnection dataConnection) => this.dataConnection = dataConnection;

        public async Task<IAtomicOperation> BeginAtomicOperationAsync() =>
            new AtomicOperation(await dataConnection.BeginTransactionAsync());

        private class AtomicOperation : IAtomicOperation
        {
            private readonly DataConnectionTransaction transaction;

            public AtomicOperation(DataConnectionTransaction transaction) =>
                this.transaction = transaction;

            public async ValueTask DisposeAsync() =>
                await transaction.DisposeAsync();

            public async Task CompleteAsync() =>
                await transaction.CommitAsync();
        }
    }
}
