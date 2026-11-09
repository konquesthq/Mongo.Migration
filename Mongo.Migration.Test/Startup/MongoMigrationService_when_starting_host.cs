using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo.Migration.Startup.DotNetCore;
using NUnit.Framework;

namespace Mongo.Migration.Test.Startup;

[TestFixture]
internal class MongoMigrationService_when_starting_host
{
    [Test]
    public async Task When_host_has_started_Then_migration_has_run()
    {
        // Arrange
        var migration = new SlowMigration();
        using var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMigration();
                services.AddTransient<IMongoMigration>(_ => migration);
            })
            .Build();

        // Act
        await host.StartAsync();

        // Assert
        migration.HasRun.Should().BeTrue();
        await host.StopAsync();
    }

    private class SlowMigration : IMongoMigration
    {
        public bool HasRun { get; private set; }

        public void Run()
        {
            Thread.Sleep(200);
            HasRun = true;
        }
    }
}
