using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Test.Migrations.Database
{
    internal class DatabaseIntegrationTest : IDisposable
    {
        private const string MigrationsCollectionName = "_migrations";

        protected IMongoClient _client;

        protected IMongoDatabase _db;

        protected IComponentRegistry _components;

        protected MongoDbRunner _mongoToGoRunner;

        protected virtual string DatabaseName { get; set; } = "DatabaseMigration";

        protected virtual string CollectionName { get; set; } = "Test";

        protected virtual void OnSetUp(DocumentVersion databaseMigrationVersion)
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);
            _db = _client.GetDatabase(DatabaseName);
            _db.CreateCollection(CollectionName);

            _components = new ComponentRegistry(new MongoMigrationSettings()
            {
                ConnectionString = _mongoToGoRunner.ConnectionString,
                Database = DatabaseName,
                DatabaseMigrationVersion = databaseMigrationVersion
            });
            _components.RegisterComponents(_client);
        }

        public void Dispose()
        {
            _mongoToGoRunner?.Dispose();
        }

        protected void InsertMigrations(IEnumerable<DatabaseMigration> migrations)
        {
            var list = migrations.Select(m => new BsonDocument { { "MigrationId", m.GetType().ToString() }, { "Version", m.Version.ToString() } });
            _db.GetCollection<BsonDocument>(MigrationsCollectionName).InsertManyAsync(list).Wait();
        }

        protected List<MigrationHistory> GetMigrationHistory()
        {
            var migrationHistoryCollection = _db.GetCollection<MigrationHistory>(MigrationsCollectionName);
            return migrationHistoryCollection.Find(m => true).ToList();
        }
    }
}