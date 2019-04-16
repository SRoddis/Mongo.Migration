using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal interface ICollectionMigrationRunner : IMigrationRunner
    {
        void RunAll();
    }
}