using System;
using System.Collections.Generic;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Services.Initializers;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Init MongoDB
            var runner = MongoDbRunner.StartForDebugging();
            var client = new MongoClient(runner.ConnectionString);

            // Init MongoMigration
            MongoMigration.Initialize();

            client.GetDatabase("TestCars").DropCollection("Car");

            // Insert old and new version of cars into MongoDB
            var cars = new List<BsonDocument>
            {
                new BsonDocument {{"Dors", 3}, {"Type", "Cabrio"}, {"UnnecessaryField", ""}},
                new BsonDocument {{"Dors", 5}, {"Type", "Combi"}, {"UnnecessaryField", ""}},
                new BsonDocument {{"Doors", 3}, {"Type", "Truck"}, {"UnnecessaryField", ""}, {"Version", "0.0.1"}},
                new BsonDocument {{"Doors", 5}, {"Type", "Van"}, {"Version", "0.1.1"}}
            };

            var bsonCollection =
                client.GetDatabase("TestCars").GetCollection<BsonDocument>("Car");

           bsonCollection.InsertManyAsync(cars).Wait();

            Console.WriteLine("Migrate from:");
            cars.ForEach(c => Console.WriteLine(c.ToBsonDocument() + "\n"));

            // Migrate old version to current version by reading collection
            var typedCollection = client.GetDatabase("TestCars").GetCollection<Car>("Car");
            var result = typedCollection.FindAsync(_ => true).Result.ToListAsync().Result;

            Console.WriteLine("To:");
            result.ForEach(r => Console.WriteLine(r.ToBsonDocument() + "\n"));

            // Create new car and add it with current version number into MongoDB
            var id = ObjectId.GenerateNewId();
            var type = "Test" + id;
            var car = new Car {Doors = 2, Type = type};

            typedCollection.InsertOne(car);
            var test = typedCollection.FindAsync(Builders<Car>.Filter.Eq(c => c.Type, type)).Result.Single();



            var aggregate = typedCollection.Aggregate()
                .Match(new BsonDocument {{"Dors", 3}});
            var results = aggregate.ToListAsync().Result;

            Console.WriteLine("New Car was created with version: " + test.Version);
            Console.WriteLine("\n");

            Console.WriteLine("\n");
            Console.WriteLine("Press any Key to exit...");
            Console.Read();

            client.GetDatabase("TestCars").DropCollection("Car");
        }
    }
}