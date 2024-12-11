using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
{
    private readonly IDatabaseVersionService _databaseVersionService;

    private readonly ILogger _logger;

    private readonly Type DatabaseMigrationType = typeof(DatabaseMigration);

    private IDatabaseTypeMigrationDependencyLocator _migrationLocator { get; }

    public DatabaseMigrationRunner(
        IDatabaseTypeMigrationDependencyLocator migrationLocator,
        IDatabaseVersionService databaseVersionService)
        : this(migrationLocator, databaseVersionService, NullLoggerFactory.Instance)
    {
    }

    private DatabaseMigrationRunner(
        IDatabaseTypeMigrationDependencyLocator migrationLocator,
        IDatabaseVersionService databaseVersionService,
        ILoggerFactory loggerFactory)
    {
        _migrationLocator = migrationLocator;
        _databaseVersionService = databaseVersionService;
        _logger = loggerFactory.CreateLogger<DatabaseMigrationRunner>();
    }

    public void Run(IMongoDatabase db)
    {
        _logger.LogInformation("Database migration started.");
        var databaseVersion = _databaseVersionService.GetLatestDatabaseVersion(db);
        var currentOrLatest = _databaseVersionService.GetCurrentOrLatestMigrationVersion();

        if (databaseVersion == currentOrLatest)
        {
            return;
        }

        MigrateUpOrDown(db, databaseVersion, currentOrLatest);
        _logger.LogInformation("Database migration finished.");
    }

    private void MigrateUpOrDown(
        IMongoDatabase db,
        DocumentVersion databaseVersion,
        DocumentVersion to)
    {
        if (databaseVersion > to)
        {
            MigrateDown(db, databaseVersion, to);
            return;
        }

        MigrateUp(db, databaseVersion, to);
    }

    private void MigrateUp(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromTo(DatabaseMigrationType, currentVersion, toVersion)
            .ToList();

        foreach (var migration in migrations)
        {
            _logger.LogInformation(
                "Database Migration Up: {0}:{Version} ", currentVersion.GetType().ToString(), migration.Version
            );

            migration.Up(db);
            _databaseVersionService.Save(db, migration);

            _logger.LogInformation(
                "Database Migration Up finished successful: {0}:{Version} ",
                migration.GetType().ToString(),
                migration.Version
            );
        }
    }

    private void MigrateDown(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
            .GetMigrationsGtEq(DatabaseMigrationType, toVersion)
            .OrderByDescending(m => m.Version)
            .Where(m => m.Version != toVersion)
            .ToList();

        foreach (var migration in migrations)
        {
            _logger.LogInformation(
                "Database Migration Down: {0}:{Version} ", migration.GetType().ToString(), migration.Version
            );

            migration.Down(db);
            _databaseVersionService.Remove(db, migration);

            _logger.LogInformation(
                "Database Migration Down finished successful: {0}:{Version} ",
                migration.GetType().ToString(),
                migration.Version
            );
        }
    }
}