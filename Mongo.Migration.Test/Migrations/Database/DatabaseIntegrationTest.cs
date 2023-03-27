using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Test.Migrations.Database
{
    internal class DatabaseIntegrationTest : IDisposable
    {
        private const string MigrationsCollectionName = "_migrations";

        protected IMongoClient Client;
        protected ServiceProvider ServiceProvider;
        protected IMongoDatabase Db;
        protected MongoDbRunner MongoToGoRunner;

        protected virtual string DatabaseName { get; set; } = "DatabaseMigration";

        protected virtual string CollectionName { get; set; } = "Test";

        public void Dispose()
        {
            MongoToGoRunner?.Dispose();
            ServiceProvider.Dispose();
        }

        protected virtual void OnSetUp(DocumentVersion databaseMigrationVersion)
        {
            var databaseName = $"{DatabaseName}-{Guid.NewGuid()}";
            MongoToGoRunner = MongoDbRunner.Start();
            Client = new MongoClient(MongoToGoRunner.ConnectionString);
            Db = Client.GetDatabase(databaseName);
            Db.CreateCollection(CollectionName);

            var serviceCollection = new ServiceCollection()
                .AddScoped<IMongoClient>(_ => Client)
                .AddMigration(x =>
                {
                    x.ConnectionString = MongoToGoRunner.ConnectionString;
                    x.Database = databaseName;
                    x.DatabaseMigrationVersion = databaseMigrationVersion;
                });
            
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected void InsertMigrations(IEnumerable<DatabaseMigration> migrations)
        {
            var list = migrations.Select(m => new BsonDocument
            {
                { "MigrationId", m.GetType().ToString() }, 
                { "Version", m.Version.ToString() }
            });
            Db.GetCollection<BsonDocument>(MigrationsCollectionName).InsertManyAsync(list).Wait();
        }

        protected List<MigrationHistory> GetMigrationHistory()
        {
            var migrationHistoryCollection = Db.GetCollection<MigrationHistory>(MigrationsCollectionName);
            return migrationHistoryCollection.Find(m => true).ToList();
        }
    }
}