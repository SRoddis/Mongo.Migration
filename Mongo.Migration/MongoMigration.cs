using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;

namespace Mongo.Migration
{
    internal class MongoMigration : IMongoMigration
    {
        private readonly IMigrationLocator _migrationLocator;
        private readonly IVersionLocator _versionLocator;
        private readonly IMigrationStrategy _migrationStrategy;

        public MongoMigration(IMigrationLocator migrationLocator, IVersionLocator versionLocator,
            IMigrationStrategy migrationStrategy)
        {
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
            _migrationStrategy = migrationStrategy;
        }

        public void Run()
        {
            _migrationLocator.LoadMigrations();
            _versionLocator.Locate();
            
            _migrationStrategy.Migrate();
        }
    }
}