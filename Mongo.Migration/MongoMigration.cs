using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration
{
    internal class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly IDatabaseTypeMigrationDependencyLocator _databaseMigrationLocator;
        private readonly IMigrationLocator<IDocumentMigration> _documentMigrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly IRuntimeVersionLocator _runtimeVersionLocator;
        private readonly IStartUpVersionLocator _startUpVersionLocator;

        public MongoMigration(
            IMigrationLocator<IDocumentMigration> documentMigrationLocator,
            IDatabaseTypeMigrationDependencyLocator databaseMigrationLocator,
            IRuntimeVersionLocator runtimeVersionLocator,
            ICollectionLocator collectionLocator,
            IStartUpVersionLocator startUpVersionLocator,
            IMigrationService migrationService)
        {
            _documentMigrationLocator = documentMigrationLocator;
            _databaseMigrationLocator = databaseMigrationLocator;
            _runtimeVersionLocator = runtimeVersionLocator;
            _collectionLocator = collectionLocator;
            _startUpVersionLocator = startUpVersionLocator;
            _migrationService = migrationService;
        }

        public void Run()
        {
            _documentMigrationLocator.Locate();
            _databaseMigrationLocator.Locate();
            _runtimeVersionLocator.Locate();
            _collectionLocator.Locate();
            _startUpVersionLocator.Locate();
            _migrationService.Migrate();
        }
    }
}