using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.Migrations.Migrations;


var serviceProvider = CreateServices();
using var scope = serviceProvider.CreateScope();
UpdateDatabase(scope.ServiceProvider);
static IServiceProvider CreateServices() =>
    new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddPostgres()
            .WithGlobalConnectionString(Environment.GetEnvironmentVariable("PG_CONNECTION_STRING"))
            .ScanIn(typeof(InitDatabase).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);

static void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}
