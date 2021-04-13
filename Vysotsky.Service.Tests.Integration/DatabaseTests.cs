﻿using System;
using LinqToDB;
using LinqToDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.Data;
using Vysotsky.Migrations;

namespace Vysotsky.Service.Tests.Integration
{
    public abstract class DatabaseTests : IDisposable
    {
        protected readonly VysotskyDataConnection Database;

        protected DatabaseTests()
        {
            var connectionString = "User ID=postgres;Password=postgres;Host=localhost;Port=5433;Database=vysotsky";

            var serviceProvider = Migrator.CreateServices(connectionString);
            using (serviceProvider.CreateScope()) Migrator.UpdateDatabase(serviceProvider);

            var options = new LinqToDbConnectionOptionsBuilder()
                .UsePostgreSQL(connectionString)
                .Build<VysotskyDataConnection>();
            Database = new VysotskyDataConnection(options);
            DropDatabase();
        }

        public void Dispose()
        {
            DropDatabase();
            Database.Dispose();
        }

        private void DropDatabase()
        {
            Database.Buildings.Delete();
            Database.Users.Delete();
            Database.BlockedTokens.Delete();
        }
    }
}
