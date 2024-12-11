using LightInject;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    internal class ComponentRegistry : IComponentRegistry
    {
        private readonly IContainerAdapter _containerAdapter;

        private readonly IMongoMigrationSettings _settings;

        public ComponentRegistry(IMongoMigrationSettings settings, IContainerAdapter containerAdapter = null)
        {
            _settings = settings;

            if (containerAdapter == null)
            {
                containerAdapter = new LightInjectAdapter(new ServiceContainer());
            }

            _containerAdapter = containerAdapter;
        }

        public void RegisterComponents(IMongoClient client)
        {
            RegisterDefaults();

            _containerAdapter.RegisterInstance<IMongoClient>(client);

            _containerAdapter.Register<IMigrationService, MigrationService>();
        }

        public TComponent Get<TComponent>()
            where TComponent : class
        {
            return (TComponent)_containerAdapter.GetInstance(typeof(TComponent));
        }

        private void RegisterDefaults()
        {
            _containerAdapter.RegisterInstance<IContainerProvider>(_containerAdapter);
            _containerAdapter.Register(typeof(IMigrationLocator<>), typeof(TypeMigrationDependencyLocator<>));
            _containerAdapter.RegisterInstance<IMongoMigrationSettings>(_settings);
            _containerAdapter.RegisterSingleton<ICollectionLocator, CollectionLocator>();
            _containerAdapter.RegisterSingleton<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>();
            _containerAdapter.RegisterSingleton<IStartUpVersionLocator, StartUpVersionLocator>();
            _containerAdapter.Register<IDatabaseVersionService, DatabaseVersionService>();
            _containerAdapter.Register<IStartUpDatabaseMigrationRunner, StartUpDatabaseMigrationRunner>();
            _containerAdapter.Register<IDatabaseMigrationRunner, DatabaseMigrationRunner>();
            _containerAdapter.Register<IMongoMigration, MongoMigration>();
        }
    }
}