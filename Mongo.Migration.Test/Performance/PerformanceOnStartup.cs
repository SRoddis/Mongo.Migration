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
            this.InsertMany(DOCUMENT_COUNT, false);
            var sw = new Stopwatch();
            sw.Start();
            this.MigrateAll(false);
            sw.Stop();

            this.ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            this.InsertMany(DOCUMENT_COUNT, true);
            var swWithMigration = new Stopwatch();
            swWithMigration.Start();
            MongoMigrationClient.Initialize(this._client);
            swWithMigration.Stop();

            this.ClearCollection();

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            Console.WriteLine(
                $"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {TOLERANCE_MS}ms), Documents: {DOCUMENT_COUNT}, Migrations per Document: 2");

            // Assert
            result.Should().BeLessThan(TOLERANCE_MS);
        }

        #region PRIVATE

        private const int DOCUMENT_COUNT = 10000;

        private const string DATABASE_NAME = "PerformanceTest";

        private const string COLLECTION_NAME = "Test";

        private const int TOLERANCE_MS = 2800;

        private MongoClient _client;

        private MongoDbRunner _runner;

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

            this._client.GetDatabase(DATABASE_NAME).GetCollection<BsonDocument>(COLLECTION_NAME).InsertManyAsync(documents)
                .Wait();
        }

        private void MigrateAll(bool withVersion)
        {
            if (withVersion)
            {
                var versionedCollectin = this._client.GetDatabase(DATABASE_NAME)
                    .GetCollection<TestDocumentWithTwoMigrationHighestVersion>(COLLECTION_NAME);
                var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
                return;
            }

            var collection = this._client.GetDatabase(DATABASE_NAME)
                .GetCollection<TestClass>(COLLECTION_NAME);
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        private void AddDocumentsToCache()
        {
            this.InsertMany(DOCUMENT_COUNT, false);
            this.MigrateAll(false);
        }

        private void ClearCollection()
        {
            this._client.GetDatabase(DATABASE_NAME).DropCollection(COLLECTION_NAME);
        }

        #endregion
    }
}