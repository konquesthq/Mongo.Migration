using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using Mongo.Migration.Startup.Static;
using Mongo.Migration.Test.TestDoubles;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Performance
{
    [TestFixture]
    public class PerformanceTestOnStartup
    {
        private const int DocumentCount = 10000;
        private const string DatabaseName = "PerformanceTest";
        private const string CollectionName = "Test";
        private const int ToleranceMs = 2800;
        private MongoClient _client;
        private MongoDbRunner _runner;

        [TearDown]
        public void TearDown()
        {
            MongoMigrationClient.Reset();
            this._client = null;
            this._runner.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            this._runner = MongoDbRunner.Start();
            this._client = new MongoClient(this._runner.ConnectionString);
        }

        [Test]
        public void When_migrating_number_of_documents()
        {
            // Arrange
            // Worm up MongoCache
            this.ClearCollection();
            this.AddDocumentsToCache();
            this.ClearCollection();

            // Act
            // Measure time of MongoDb processing without Mongo.Migration
            this.InsertMany(DocumentCount, false);
            var sw = new Stopwatch();
            sw.Start();
            this.MigrateAll();
            sw.Stop();

            this.ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            this.InsertMany(DocumentCount, true);
            var swWithMigration = new Stopwatch();
            swWithMigration.Start();
            MongoMigrationClient.Initialize(this._client);
            swWithMigration.Stop();

            this.ClearCollection();

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            Console.WriteLine(
                $"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {ToleranceMs}ms), Documents: {DocumentCount}, Migrations per Document: 2");

            // Assert
            result.Should().BeLessThan(ToleranceMs);
        }

        private void InsertMany(int number, bool withVersion)
        {
            var documents = new List<BsonDocument>();
            for (var n = 0; n < number; n++)
            {
                var document = new BsonDocument
                {
                    { "Dors", 3 }
                };
                if (withVersion)
                {
                    document.Add("Version", "0.0.0");
                }

                documents.Add(document);
            }

            this._client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(CollectionName).InsertManyAsync(documents)
                .Wait();
        }

        private void MigrateAll()
        {
            var collection = this._client.GetDatabase(DatabaseName)
                .GetCollection<TestClass>(CollectionName);
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        private void AddDocumentsToCache()
        {
            this.InsertMany(DocumentCount, false);
            this.MigrateAll();
        }

        private void ClearCollection()
        {
            this._client.GetDatabase(DatabaseName).DropCollection(CollectionName);
        }
    }
}