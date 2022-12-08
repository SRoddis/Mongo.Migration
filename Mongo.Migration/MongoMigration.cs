using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;

namespace Mongo.Migration
{
    internal class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly IStartUpVersionLocator _startUpVersionLocator;
        private readonly IMigrationLocator<IDocumentMigration> _documentMigrationLocator;
        private readonly IDatabaseTypeMigrationDependencyLocator _databaseMigrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly IRuntimeVersionLocator _runtimeVersionLocator;
        private readonly IMongoMigrationSettings _settings;

        public MongoMigration(
            IMigrationLocator<IDocumentMigration> documentMigrationLocator,
            IDatabaseTypeMigrationDependencyLocator databaseMigrationLocator,
            IRuntimeVersionLocator runtimeVersionLocator,
            ICollectionLocator collectionLocator,
            IStartUpVersionLocator startUpVersionLocator,
            IMigrationService migrationService,
            IMongoMigrationSettings settings)
        {
            _documentMigrationLocator = documentMigrationLocator;
            _databaseMigrationLocator = databaseMigrationLocator;
            _runtimeVersionLocator = runtimeVersionLocator;
            _collectionLocator = collectionLocator;
            _startUpVersionLocator = startUpVersionLocator;
            _migrationService = migrationService;
            _settings = settings;
        }

        public void Run()
        {
            if (!_settings.SkipDocumentMigration)
            {
                _documentMigrationLocator.Locate();
            }
            _databaseMigrationLocator.Locate();
            _runtimeVersionLocator.Locate();
            if(!_settings.SkipDocumentMigration)
            {
                _collectionLocator.Locate();
            }

            _startUpVersionLocator.Locate();

            _migrationService.Migrate();
        }
    }
}