using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    public interface IDatabaseMigration : IMigration
    {
        void Up(IMongoDatabase db);

        void Down(IMongoDatabase db);
    }
}