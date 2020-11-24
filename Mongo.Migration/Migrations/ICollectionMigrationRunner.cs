using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    public interface ICollectionMigrationRunner
    {
        void RunAll();
    }
}