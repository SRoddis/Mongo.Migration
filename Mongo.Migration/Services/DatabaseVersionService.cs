using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup;
using MongoDB.Driver;

namespace Mongo.Migration.Services
{
    internal class DatabaseVersionService : IDatabaseVersionService
    {
        private const string MigrationsCollectionName = "_migrations";
        private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;
        private readonly IMongoMigrationSettings _mongoMigrationSettings;

        public DatabaseVersionService(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IMongoMigrationSettings mongoMigrationSettings)
        {
            _migrationLocator = migrationLocator;
            _mongoMigrationSettings = mongoMigrationSettings;
        }

        public DocumentVersion GetCurrentOrLatestMigrationVersion()
        {
            return _mongoMigrationSettings.DatabaseMigrationVersion > DocumentVersion.Empty()
                       ? _mongoMigrationSettings.DatabaseMigrationVersion
                       : _migrationLocator.GetLatestVersion(typeof(DatabaseMigration));
        }

        public DocumentVersion GetLatestDatabaseVersion(IMongoDatabase db)
        {
            var migrations = GetMigrationsCollection(db).Find(m => true).ToList();
            if (migrations == null || !migrations.Any())
            {
                return DocumentVersion.Default();
            }

            return migrations.Max(m => m.Version);
        }

        public void Save(IMongoDatabase db, IDatabaseMigration migration)
        {
            GetMigrationsCollection(db).InsertOne(
                new MigrationHistory
                {
                    MigrationId = migration.GetType().ToString(),
                    Version = migration.Version
                });
        }

        public void Remove(IMongoDatabase db, IDatabaseMigration migration)
        {
            GetMigrationsCollection(db).DeleteOne(Builders<MigrationHistory>.Filter.Eq(mh => mh.MigrationId, migration.GetType().ToString()));
        }

        private IMongoCollection<MigrationHistory> GetMigrationsCollection(IMongoDatabase db)
        {
            return db.GetCollection<MigrationHistory>(MigrationsCollectionName);
        }
    }
}