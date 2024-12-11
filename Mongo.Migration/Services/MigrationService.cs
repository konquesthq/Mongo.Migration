using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Services;

internal class MigrationService(IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner) : IMigrationService
{
    public void Migrate()
    {
        OnStartup();
    }

    private void OnStartup()
    {
        startUpDatabaseMigrationRunner.RunAll();
    }
}