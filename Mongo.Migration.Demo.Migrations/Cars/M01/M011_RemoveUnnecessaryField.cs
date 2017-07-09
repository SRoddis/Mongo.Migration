using System;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.MongoMigrations.Cars.M01
{
    [MigrationMarker]
    public class M011_RemoveUnnecessaryField : Migration<Car>
    {
        public M011_RemoveUnnecessaryField()
            : base("0.1.1")
        {
        }

        public override void Up(BsonDocument document)
        {
            document.Remove("UnnecessaryField");
        }

        public override void Down(BsonDocument document)
        {
            document.Add("UnnecessaryField", "");
        }
    }
}