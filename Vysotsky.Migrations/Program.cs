using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.Migrations;
using Vysotsky.Migrations.Migrations;


var serviceProvider = Migrator.CreateServices(Environment.GetEnvironmentVariable("PG_CONNECTION_STRING"));
using var scope = serviceProvider.CreateScope();
Migrator.UpdateDatabase(scope.ServiceProvider);

namespace Vysotsky.Migrations
{
    public static class Migrator
    {
        public static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(InitDatabase).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        public static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
