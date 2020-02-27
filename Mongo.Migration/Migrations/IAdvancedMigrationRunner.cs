using Mongo.Migration.Migrations.Locators;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal interface IAdvancedMigrationRunner
    {
        IAdvancedMigrationLocator _migrationLocator { get; }
        
        void Run(IMongoDatabase db, string runnigVersion);
    }
}