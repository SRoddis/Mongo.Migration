using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
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
    }
}
