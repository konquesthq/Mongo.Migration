using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration;

internal class MongoMigration : IMongoMigration
{
    private readonly ICollectionLocator _collectionLocator;
    private readonly IDatabaseTypeMigrationDependencyLocator _databaseMigrationLocator;
    private readonly IMigrationService _migrationService;
    private readonly IStartUpVersionLocator _startUpVersionLocator;

    public MongoMigration(
        IDatabaseTypeMigrationDependencyLocator databaseMigrationLocator,
        ICollectionLocator collectionLocator,
        IStartUpVersionLocator startUpVersionLocator,
        IMigrationService migrationService)
    {
        _databaseMigrationLocator = databaseMigrationLocator;
        _collectionLocator = collectionLocator;
        _startUpVersionLocator = startUpVersionLocator;
        _migrationService = migrationService;
    }

    public void Run()
    {
        _databaseMigrationLocator.Locate();
        _collectionLocator.Locate();
        _startUpVersionLocator.Locate();
        _migrationService.Migrate();
    }
}