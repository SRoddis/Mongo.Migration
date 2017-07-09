using System;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.MongoMigrations.Cars.M00
{
    [MigrationMaker]
    public class M001_RenameDorsToDoors : Migration<Car>
    {
        public M001_RenameDorsToDoors()
            : base("0.0.1")
        {
        }

        public override void Up(BsonDocument document)
        {
            var doors = document["Dors"].ToInt32();
            document.Add("Doors", doors);
            document.Remove("Dors");
        }

        public override void Down(BsonDocument document)
        {
            var doors = document["Doors"].ToInt32();
            document.Add("Dors", doors);
            document.Remove("Doors");
        }
    }
}