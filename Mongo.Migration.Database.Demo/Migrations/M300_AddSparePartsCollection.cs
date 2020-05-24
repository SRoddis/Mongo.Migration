using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Database.Demo.Migrations
{
    public class M300_AddSparePartsCollection : DatabaseMigration
    {
        public M300_AddSparePartsCollection()
            : base("3.0.0")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            db.CreateCollection(nameof(SparePart));
        }

        public override void Down(IMongoDatabase db)
        {
            db.DropCollection(nameof(SparePart));
        }
    }
}