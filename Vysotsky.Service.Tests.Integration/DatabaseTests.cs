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
        protected readonly VysotskyDataConnection ____RULE_VIOLATION____Database____RULE_VIOLATION____;

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
            this.____RULE_VIOLATION____Database____RULE_VIOLATION____ = new VysotskyDataConnection(options);
            DropDatabase();
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

                DropDatabase();
                this.____RULE_VIOLATION____Database____RULE_VIOLATION____.Dispose();
                this.disposed = true;
            }
        }

        private void DropDatabase()
        {
            this.____RULE_VIOLATION____Database____RULE_VIOLATION____.Buildings.Delete();
            this.____RULE_VIOLATION____Database____RULE_VIOLATION____.Users.Delete();
            this.____RULE_VIOLATION____Database____RULE_VIOLATION____.BlockedTokens.Delete();
        }
    }
}
