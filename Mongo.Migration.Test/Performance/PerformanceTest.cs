using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using Mongo.Migration.Services.Initializers;
using Mongo.Migration.Test.TestDoubles;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Performance
{
    [TestFixture]
    public class PerformanceTest
    {
        [TearDown]
        public void TearDown()
        {
            MongoMigration.Reset();
        }

        [SetUp]
        public void SetUp()
        {
            var runner = MongoDbRunner.StartForDebugging();
            _client = new MongoClient(runner.ConnectionString);
        }

        private MongoClient _client;

        private void InsertMany(int number, bool withVersion)
        {
            var documents = new List<BsonDocument>();
            for (var n = 0; n < number; n++)
            {
                var document = new BsonDocument
                {
                    {"Dors", 3}
                };
                if (withVersion)
                    document.Add("Version", "0.0.0");
                documents.Add(document);
            }

            _client.GetDatabase("PerformanceTest").GetCollection<BsonDocument>("Tests").InsertManyAsync(documents)
                .Wait();
        }

        private void MigrateAll(bool withVersion)
        {
            if (withVersion)
            {
                var versionedCollectin = _client.GetDatabase("PerformanceTest")
                    .GetCollection<TestDocumentWithTwoMigrationHighestVersion>("Tests");
                var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
                return;
            }

            var collection = _client.GetDatabase("PerformanceTest")
                .GetCollection<TestClass>("Tests");
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        private void AddDocumentsToCache()
        {
            InsertMany(1000, false);
            MigrateAll(false);
        }

        private void ClearCollection()
        {
            _client.GetDatabase("PerformanceTest").DropCollection("Tests");
        }

        [Test]
        public void When_migrating_number_of_documents()
        {
            // Arrange

            ClearCollection();
            AddDocumentsToCache();
            ClearCollection();

            // Act
            // Measure time of MongoDb processing without Mongo.Migration
            var sw = new Stopwatch();
            sw.Start();
            InsertMany(1000, false);
            MigrateAll(false);
            sw.Stop();
            ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            MongoMigration.Initialize();

            var swWithMigration = new Stopwatch();
            swWithMigration.Start();
            InsertMany(1000, true);
            MigrateAll(true);
            swWithMigration.Stop();

            ClearCollection();

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            Console.WriteLine(
                $"MongoDB: {sw.ElapsedMilliseconds}, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}, Diff: {result}");

            result.Should().BeLessThan(500);
        }
    }
}