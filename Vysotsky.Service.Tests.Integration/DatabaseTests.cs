using System;
using LinqToDB;
using LinqToDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.Data;
using Vysotsky.Migrations;

namespace Vysotsky.Service.Tests.Integration
{
    public abstract class DatabaseTests : IDisposable
    {
        protected VysotskyDataConnection DataConnection { get; }

        protected DatabaseTests()
        {
            var connectionString = "User ID=postgres;Password=postgres;Host=localhost;Port=5433;Database=vysotsky";

            var serviceProvider = Migrator.CreateServices(connectionString);
            using (serviceProvider.CreateScope())
            {
                Migrator.UpdateDatabase(serviceProvider);
            }

            var options = new LinqToDbConnectionOptionsBuilder()
                .UsePostgreSQL(connectionString)
                .Build<VysotskyDataConnection>();
            DataConnection = new VysotskyDataConnection(options);
            DropDatabase();
        }

        private bool disposed;
        private readonly object _disposeLock = new();

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            lock (_disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                DropDatabase();
                DataConnection.Dispose();
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        private void DropDatabase()
        {
            DataConnection.Buildings.Delete();
            DataConnection.Users.Delete();
            DataConnection.BlockedTokens.Delete();
        }
    }
}
