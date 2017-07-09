using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Services.Initializers;
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

            _client.GetDatabase("PerformanceTestCar").GetCollection<BsonDocument>("Tests").InsertManyAsync(documents)
                .Wait();
        }

        private static void MigrateAll(bool withVersion)
        {
            if (withVersion)
            {
                var versionedCollectin = _client.GetDatabase("PerformanceTestCar")
                    .GetCollection<Car>("Tests");
                var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
                return;
            }

            var collection = _client.GetDatabase("PerformanceTestCar")
                .GetCollection<CarWithoutVersion>("Tests");
            var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
        }

        private static void AddDocumentsToCache()
        {
            InsertMany(10000, false);
            MigrateAll(false);
        }

        private static void ClearCollection()
        {
            _client.GetDatabase("PerformanceTestCar").DropCollection("Tests");
        }

        private static void Main(string[] args)
        {
            // Worm up MongoCache
            ClearCollection();
            AddDocumentsToCache();
            ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            var sw = new Stopwatch();
            sw.Start();
            InsertMany(10000, false);
            MigrateAll(false);
            sw.Stop();
            ClearCollection();

            // Measure time of MongoDb processing without Mongo.Migration
            MongoMigration.Initialize();

            var swWithMigration = new Stopwatch();
            swWithMigration.Start();
            InsertMany(10000, true);
            MigrateAll(true);
            swWithMigration.Stop();

            ClearCollection();

            var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

            System.Console.WriteLine(
                $"MongoDB: {sw.ElapsedMilliseconds}, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}, Diff: {result}");
        }
    }
}