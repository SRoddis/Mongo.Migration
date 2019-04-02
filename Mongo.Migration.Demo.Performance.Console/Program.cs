using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Services.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.Performance.Console
{
    internal class Program
    {
        private static readonly MongoClient _client;

        static Program()
        {
            var runner = MongoDbRunner.StartForDebugging();
            _client = new MongoClient(runner.ConnectionString);
        }

        #region private

        private const int DOCUMENT_COUNT = 10000;

        private const string DATABASE_NAME = "PerformanceTestCar";

        private const string COLLECTION_NAME = "Test";

        private const int TOLERANCE_MS = 200;

        private static void InsertMany(int number, bool withVersion)
        {
            var documents = new List<BsonDocument>();
            for (var n = 0; n < number; n++)
            {
                var document = new BsonDocument
                {
                    {"Dors", 3}
                };
                if (withVersion)
                {
                    document.Add("Version", "0.0.0");
                    document.Add("Type", "Truck");
                }
                
                documents.Add(document);
            }

            _client.GetDatabase(DATABASE_NAME).GetCollection<BsonDocument>(COLLECTION_NAME).InsertManyAsync(documents)
                .Wait();
        }

        private static void MigrateAll(bool withVersion)
        {
            if (withVersion)
            {
                var versionedCollectin = _client.GetDatabase(DATABASE_NAME)
                    .GetCollection<Car>(COLLECTION_NAME);
                var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
                return;
            }

            var collection = _client.GetDatabase(DATABASE_NAME)
                .GetCollection<CarWithoutVersion>(COLLECTION_NAME);
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        private static void AddDocumentsToCache()
        {
            InsertMany(DOCUMENT_COUNT, false);
            MigrateAll(false);
        }

        private static void ClearCollection()
        {
            _client.GetDatabase(DATABASE_NAME).DropCollection(COLLECTION_NAME);
        }

        #endregion

        private static void Main(string[] args)
        {
            // Arrange
            // Worm up MongoCache
            ClearCollection();
            AddDocumentsToCache();
            ClearCollection();

            // Act
            // Measure time of MongoDb processing without Mongo.Migration
            var sw = new Stopwatch();
            sw.Start();
            InsertMany(DOCUMENT_COUNT, false);
            MigrateAll(false);
            sw.Stop();

            ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            MongoMigration.MigrationOnDeserialization();

            var swWithMigration = new Stopwatch();
            swWithMigration.Start();
            InsertMany(DOCUMENT_COUNT, true);
            MigrateAll(true);
            swWithMigration.Stop();

            ClearCollection();

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            System.Console.WriteLine(
                $"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {TOLERANCE_MS}ms), Documents: {DOCUMENT_COUNT}, Migrations per Document: 2");

            System.Console.ReadLine();
        }
    }
}