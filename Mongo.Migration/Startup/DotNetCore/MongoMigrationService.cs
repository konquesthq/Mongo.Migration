using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup.DotNetCore;

public class MongoMigrationService(IMongoMigration mongoMigration, ILoggerFactory loggerFactory)
    : BackgroundService
{
    private readonly ILogger<MongoMigrationService> _logger = loggerFactory.CreateLogger<MongoMigrationService>();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Running migration. Please wait....");
            mongoMigration.Run();
            _logger.LogInformation("Migration has been done");
        }
        catch (Exception ex)
        {
            var exType = ex.GetType().ToString();
            _logger.LogError(ex, "Error during migration: {ExType}", exType);
        }
        
        return Task.CompletedTask;
    }
}