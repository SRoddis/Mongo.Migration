using Mongo.Migration.Demo.Model;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.MongoMigrations.Cars.M00
{
    public class M100_AddNewCar : DatabaseMigration
    {
        public M100_AddNewCar()
            : base("1.0.0")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            var collection = db.GetCollection<Car>("Car");
            collection.InsertOne(new Car
            {
                Doors = 123,
                Type = "AddedInMigration"
            });
        }

        public override void Down(IMongoDatabase db)
        {
        }
    }
}