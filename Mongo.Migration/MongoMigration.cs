using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration
{
    internal class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly IMigrationLocator _migrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly IVersionLocator _versionLocator;

        public MongoMigration(IMigrationLocator migrationLocator, IVersionLocator versionLocator,
            ICollectionLocator collectionLocator, IMigrationService migrationService)
        {
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
            _collectionLocator = collectionLocator;
            _migrationService = migrationService;
        }

        public void Run()
        {
            _migrationLocator.Locate();
            _versionLocator.Locate();
            _collectionLocator.Locate();

            _migrationService.Migrate();
        }
    }
}