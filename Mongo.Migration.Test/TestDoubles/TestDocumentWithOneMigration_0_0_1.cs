using System;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Test.TestDoubles
{
    [MigrationMaker]
    internal class TestDocumentWithOneMigration_0_0_1 : Migration<TestDocumentWithOneMigration>
    {
        public TestDocumentWithOneMigration_0_0_1() : base("0.0.1")
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