using System;
using Mongo.Migration.Demo.Model;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.MongoMigrations.Cars.M01
{
    public class M011_RemoveUnnecessaryField : DocumentMigration<Car>
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