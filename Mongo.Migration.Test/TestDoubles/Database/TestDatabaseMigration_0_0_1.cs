using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Test.TestDoubles
{
    internal class TestDatabaseMigration_0_0_1 : DatabaseMigration
    {
        public TestDatabaseMigration_0_0_1()
            : base("0.0.1")
        {
        }

        public override void Up(IMongoDatabase db)
        {
        }

        public override void Down(IMongoDatabase db)
        {
        }
    }
}