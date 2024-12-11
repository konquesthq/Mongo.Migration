using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration.Startup.DotNetCore;

public static class MongoMigrationExtensions
{
    public static void AddMigration(this IServiceCollection services, IMongoMigrationSettings settings = null)
    {
        RegisterDefaults(services, settings ?? new MongoMigrationSettings());

        services.AddScoped<IMigrationService, MigrationService>();
    }

    private static void RegisterDefaults(IServiceCollection services, IMongoMigrationSettings settings)
    {
        services.AddSingleton(settings);

        services.AddSingleton<IContainerProvider, Migrations.Adapters.ServiceProvider>();
        services.AddSingleton(typeof(IMigrationLocator<>), typeof(TypeMigrationDependencyLocator<>));
        services.AddSingleton<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>();
        services.AddSingleton<ICollectionLocator, CollectionLocator>();
        services.AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>();
        services.AddTransient<IDatabaseVersionService, DatabaseVersionService>();
        services.AddTransient<IStartUpDatabaseMigrationRunner, StartUpDatabaseMigrationRunner>();
        services.AddTransient<IDatabaseMigrationRunner, DatabaseMigrationRunner>();
        services.AddTransient<IMongoMigration, MongoMigration>();
    }
}