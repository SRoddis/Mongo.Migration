using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration
{
    public class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly IStartUpVersionLocator _startUpVersionLocator;
        private readonly IMigrationLocator _migrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly IRuntimeVersionLocator _runtimeVersionLocator;

        public MongoMigration(IMigrationLocator migrationLocator, IRuntimeVersionLocator runtimeVersionLocator,
            ICollectionLocator collectionLocator, IStartUpVersionLocator startUpVersionLocator, IMigrationService migrationService)
        {
            _migrationLocator = migrationLocator;
            _runtimeVersionLocator = runtimeVersionLocator;
            _collectionLocator = collectionLocator;
            _startUpVersionLocator = startUpVersionLocator;
            _migrationService = migrationService;
        }

        public void Run()
        {
            _migrationLocator.Locate();
            _runtimeVersionLocator.Locate();
            _collectionLocator.Locate();
            _startUpVersionLocator.Locate();

            _migrationService.Migrate();
        }
    }
}