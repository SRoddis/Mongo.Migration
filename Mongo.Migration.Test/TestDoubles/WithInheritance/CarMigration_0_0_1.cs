using Mongo.Migration.Migrations;
using MongoDB.Bson;

namespace Mongo.Migration.Test.TestDoubles
{
    internal class CarMigration_0_0_1 : Migration<Car>
    {
        public CarMigration_0_0_1() : base("0.0.1")
        {
        }

        public override void Up(BsonDocument document)
        {
        }

        public override void Down(BsonDocument document)
        {
        }
    }
}
