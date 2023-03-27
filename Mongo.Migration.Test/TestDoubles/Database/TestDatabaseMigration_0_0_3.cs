using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Test.TestDoubles
{
    internal class TestDatabaseMigration_0_0_3 : DatabaseMigration
    {
        public TestDatabaseMigration_0_0_3()
            : base("0.0.3")
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