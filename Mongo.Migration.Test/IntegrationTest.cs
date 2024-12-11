using System;

using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;

using Mongo2Go;

using MongoDB.Driver;

namespace Mongo.Migration.Test
{
    public class IntegrationTest : IDisposable
    {
        protected IMongoClient Client;

        protected IComponentRegistry Components;

        protected MongoDbRunner MongoToGoRunner;

        public void Dispose()
        {
            MongoToGoRunner?.Dispose();
        }

        protected void OnSetUp()
        {
            MongoToGoRunner = MongoDbRunner.Start();
            Client = new MongoClient(MongoToGoRunner.ConnectionString);
            Client.GetDatabase("PerformanceTest").CreateCollection("Test");
            Components = new ComponentRegistry(
                new MongoMigrationSettings
                {
                    ConnectionString = MongoToGoRunner.ConnectionString, Database = "PerformanceTest"
                }
            );
            Components.RegisterComponents(Client);
        }
    }
}