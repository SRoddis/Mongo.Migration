using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration
{
    internal class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly ICollectionVersionLocator _collectionVersionLocator;
        private readonly IMigrationLocator _migrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly ICurrentVersionLocator _currentVersionLocator;

        public MongoMigration(IMigrationLocator migrationLocator, ICurrentVersionLocator currentVersionLocator,
            ICollectionLocator collectionLocator, ICollectionVersionLocator collectionVersionLocator, IMigrationService migrationService)
        {
            _migrationLocator = migrationLocator;
            _currentVersionLocator = currentVersionLocator;
            _collectionLocator = collectionLocator;
            _collectionVersionLocator = collectionVersionLocator;
            _migrationService = migrationService;
        }

        public void Run()
        {
            _migrationLocator.Locate();
            _currentVersionLocator.Locate();
            _collectionLocator.Locate();
            _collectionVersionLocator.Locate();

            _migrationService.Migrate();
        }
    }
}