using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Services
{
    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly IStartUpDatabaseMigrationRunner _startUpDatabaseMigrationRunner;

        public MigrationService(IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner)
            : this(NullLoggerFactory.Instance)
        {
            this._startUpDatabaseMigrationRunner = startUpDatabaseMigrationRunner;
        }

        private MigrationService(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<MigrationService>();
        }

        public void Migrate()
        {
            this.OnStartup();
        }

        private void OnStartup()
        {
            this._startUpDatabaseMigrationRunner.RunAll();
        }
    }
}