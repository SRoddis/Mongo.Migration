using Mongo.Migration.Database.Demo.Migrations;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Migration.Database.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init MongoDB
            var runner = MongoDbRunner.StartForDebugging();
            var client = new MongoClient(runner.ConnectionString);

            client.GetDatabase("TestCars").DropCollection("Car");
            client.GetDatabase("TestCars").DropCollection("_migrations");
            client.GetDatabase("TestCars").DropCollection(nameof(SparePart));
            // Insert old and new version of cars into MongoDB
            //var cars = new List<Car>
            //{
            //   new Car { Doors = 222, Type = "AddedInMigration" }
            //};

            //var bsonCollection =
            //    client.GetDatabase("TestCars").GetCollection<Car>("Car");
            //bsonCollection.InsertManyAsync(cars).Wait();
            //client.GetDatabase("TestCars").CreateCollection(nameof(SparePart));
          // InsertMigrations(client);

            // Init MongoMigration
            MongoMigrationClient.Initialize(
                client,
                new MongoMigrationSettings()
                {
                    ConnectionString = runner.ConnectionString,
                    Database = "TestCars"
                },
                new LightInjectAdapter(new LightInject.ServiceContainer()));

            Console.WriteLine("Apply database migrations: ");
            Console.WriteLine("\n");

            var migrationsCollection = client.GetDatabase("TestCars").GetCollection<BsonDocument>("_migrations");
            var migrations = migrationsCollection.FindAsync(_ => true).Result.ToListAsync().Result;
            migrations.ForEach(r => Console.WriteLine(r + "\n"));

            var carsCollection = client.GetDatabase("TestCars").GetCollection<Car>("Car");
            var addedInMigration = carsCollection.FindAsync(Builders<Car>.Filter.Eq(c => c.Type, "AddedInMigration")).Result.FirstOrDefault();

            Console.WriteLine("New Car was added and updated in database migrations: ");
            Console.WriteLine(addedInMigration?.ToBsonDocument() + "\n");

            Console.WriteLine("\n");
            Console.WriteLine("Press any Key to exit...");
            Console.Read();

            client.GetDatabase("TestCars").DropCollection("Car");
            client.GetDatabase("TestCars").DropCollection("_migrations");
        }

        //private static void InsertMigrations(MongoClient client)
        //{
        //    var addNewCarMigration = new M100_AddNewCar();
        //    var updateNewCarMigration = new M200_UpdateNewCar();
        //    var addSparePartsCollection = new M300_AddSparePartsCollection();
        //    var migrations = new List<BsonDocument>
        //    {
        //        new BsonDocument { { "MigrationId", addNewCarMigration.GetType().ToString() },{ "Version" , addNewCarMigration.Version.ToString()} },
        //        new BsonDocument { { "MigrationId", updateNewCarMigration.GetType().ToString() },{ "Version", updateNewCarMigration.Version.ToString() } },
        //        new BsonDocument { { "MigrationId", addSparePartsCollection.GetType().ToString() },{ "Version", addSparePartsCollection.Version.ToString() } },
        //    };

        //    var bsonCollection =
        //        client.GetDatabase("TestCars").GetCollection<BsonDocument>("_migrations");

        //    bsonCollection.InsertManyAsync(migrations).Wait();
        //}
    }
}
