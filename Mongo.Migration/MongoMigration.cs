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
            this._documentMigrationLocator = documentMigrationLocator;
            this._databaseMigrationLocator = databaseMigrationLocator;
            this._runtimeVersionLocator = runtimeVersionLocator;
            this._collectionLocator = collectionLocator;
            this._startUpVersionLocator = startUpVersionLocator;
            this._migrationService = migrationService;
        }

        public void Run()
        {
            this._documentMigrationLocator.Locate();
            this._databaseMigrationLocator.Locate();
            this._runtimeVersionLocator.Locate();
            this._collectionLocator.Locate();
            this._startUpVersionLocator.Locate();

            this._migrationService.Migrate();
        }
    }
}