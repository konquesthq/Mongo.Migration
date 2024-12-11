using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup.DotNetCore;

public class MongoMigrationService(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
    : BackgroundService
{
    private readonly ILogger<MongoMigrationService> _logger = loggerFactory.CreateLogger<MongoMigrationService>();
    private readonly IMongoMigration _migration =
        serviceScopeFactory.CreateScope().ServiceProvider.GetService<IMongoMigration>();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
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
}