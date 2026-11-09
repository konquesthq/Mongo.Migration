using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup.DotNetCore;

public class MongoMigrationService(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
    : IHostedService
{
    private readonly ILogger<MongoMigrationService> _logger = loggerFactory.CreateLogger<MongoMigrationService>();
    private readonly IMongoMigration _migration =
        serviceScopeFactory.CreateScope().ServiceProvider.GetService<IMongoMigration>();

    // Runs in StartAsync rather than BackgroundService.ExecuteAsync so the host doesn't finish starting
    // (and serve requests) until migrations are done. From .NET 10, ExecuteAsync no longer blocks startup.
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running migration. Please wait....");

            _migration.Run();

            _logger.LogInformation("Migration has been done");
        }
        catch (Exception ex)
        {
            var errorType = ex.GetType().ToString();
            _logger.LogError(ex, "An error occured during migration: {ErorType}", errorType);
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}