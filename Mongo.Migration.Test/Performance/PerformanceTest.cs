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
        private void InsertMany(int number, bool withVersion, MongoClient client)
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

            client.GetDatabase("PerformanceTest").GetCollection<BsonDocument>("Tests").InsertManyAsync(documents)
                .Wait();
        }

        private void MigrateAll(MongoClient client, bool withVersion)
        {
            if (withVersion)
            {
                var versionedCollectin = client.GetDatabase("PerformanceTest")
                    .GetCollection<TestDocumentWithTwoMigrationHighestVersion>("Tests");
                var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
                return;
            }

            var collection = client.GetDatabase("PerformanceTest")
                .GetCollection<TestClass>("Tests");
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        [TearDown]
        public void TearDown()
        {
            MongoMigration.Reset();
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void When_migrating_number_of_documents()
        {
            // Arrange
            var swWithMigration = new Stopwatch();
            var runner = MongoDbRunner.StartForDebugging();
            var client = new MongoClient(runner.ConnectionString);

            
            AddDocumentsToCache(client);
            client.GetDatabase("PerformanceTest").DropCollection("Tests");

            // Act
            // Measure time of MongoDb processing without Mongo.Migration
            var sw = new Stopwatch();
            sw.Start();
            InsertMany(1000, false, client);
            MigrateAll(client, false);
            sw.Stop();
            client.GetDatabase("PerformanceTest").DropCollection("Tests");

            // Measure time of MongoDb processing without Mongo.Migration
            MongoMigration.Initialize();

            swWithMigration.Start();
            InsertMany(1000, true, client);
            MigrateAll(client, true);
            swWithMigration.Stop();

            client.GetDatabase("PerformanceTest").DropCollection("Tests");

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            Console.WriteLine($"MongoDB: {sw.ElapsedMilliseconds}, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}, Diff: {result}");

            result.Should().BeLessThan(500);
        }

        private void AddDocumentsToCache(MongoClient client)
        {
            InsertMany(1000, false, client);
            MigrateAll(client, false);
        }

        private void ClearCollection(client)
        {
            client.GetDatabase("PerformanceTest").DropCollection("Tests");
        }
    }
}