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
            this.DataConnection = new VysotskyDataConnection(options);
            this.DropDatabase();
        }

        private bool disposed;
        private readonly object disposeLock = new();

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.disposeLock)
            {
                if (this.disposed)
                {
                    return;
                }

                this.DropDatabase();
                this.DataConnection.Dispose();
                this.disposed = true;
            }
        }

        private void DropDatabase()
        {
            this.DataConnection.Buildings.Delete();
            this.DataConnection.Users.Delete();
            this.DataConnection.BlockedTokens.Delete();
        }
    }
}
