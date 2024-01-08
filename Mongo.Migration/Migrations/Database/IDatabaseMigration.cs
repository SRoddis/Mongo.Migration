using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

public interface IDatabaseMigration : IMigration
{
    void Up(IMongoDatabase db);

    void Down(IMongoDatabase db);
}