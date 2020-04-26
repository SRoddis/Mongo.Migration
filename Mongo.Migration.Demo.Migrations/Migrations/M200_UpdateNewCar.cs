using Mongo.Migration.Demo.Model;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.MongoMigrations.Cars.M00
{
    public class M200_UpdateNewCar : DatabaseMigration
    {
        public M200_UpdateNewCar()
            : base("2.0.0")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            var collection = db.GetCollection<Car>("Car");
            var filter = Builders<Car>.Filter.Eq(c => c.Type, "AddedInMigration");
            var update = Builders<Car>.Update.Set(c => c.Doors, 222);
            collection.UpdateOne(filter, update);
        }

        public override void Down(IMongoDatabase db)
        {
            var collection = db.GetCollection<Car>("Car");
            var filter = Builders<Car>.Filter.Eq(c => c.Type, "AddedInMigration");
            var update = Builders<Car>.Update.Set(c => c.Doors, 123);
            collection.UpdateOne(filter, update);
        }
    }
}