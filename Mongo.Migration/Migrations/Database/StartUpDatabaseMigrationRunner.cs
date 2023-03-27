using System.Linq;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Startup;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal class StartUpDatabaseMigrationRunner : IStartUpDatabaseMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly ICollectionLocator _collectionLocator;

        private readonly string _databaseName;

        private readonly IDatabaseMigrationRunner _migrationRunner;

        public StartUpDatabaseMigrationRunner(
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IDatabaseMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                migrationRunner)
        {
            if (settings.ConnectionString == null && settings.Database == null || settings.ClientSettings == null)
            {
                throw new MongoMigrationNoMongoClientException();
            }

            _client = settings.ClientSettings != null ? new MongoClient(settings.ClientSettings) : new MongoClient(settings.ConnectionString);

            _databaseName = settings.Database;
        }

        public StartUpDatabaseMigrationRunner(
            IMongoClient client,
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IDatabaseMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                migrationRunner)
        {
            _client = client;
            if (settings.ConnectionString == null && settings.Database == null)
            {
                return;
            }

            _client = new MongoClient(settings.ConnectionString);
            _databaseName = settings.Database;
        }

        private StartUpDatabaseMigrationRunner(
            ICollectionLocator collectionLocator,
            IDatabaseMigrationRunner migrationRunner)
        {
            _collectionLocator = collectionLocator;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var locations = _collectionLocator.GetLocatesOrEmpty().ToList();
            var information = locations.FirstOrDefault().Value;
            var databaseName = GetDatabaseOrDefault(information);

            _migrationRunner.Run(_client.GetDatabase(databaseName));
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information)
        {
            if (string.IsNullOrEmpty(_databaseName) && string.IsNullOrEmpty(information.Database))
            {
                throw new NoDatabaseNameFoundException();
            }

            return string.IsNullOrEmpty(information.Database) ? _databaseName : information.Database;
        }
    }
}