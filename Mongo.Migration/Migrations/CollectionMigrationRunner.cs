using System.Collections.Generic;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class CollectionMigrationRunner : MigrationRunner, ICollectionMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly IDatabaseLocator _databaseLocator;

        public CollectionMigrationRunner(IMongoClient client, IDatabaseLocator databaseLocator,
            IMigrationLocator migrationLocator, IVersionLocator versionLocator)
            : base(migrationLocator, versionLocator)
        {
            _client = client;
            _databaseLocator = databaseLocator;
        }

        public void RunAll()
        {
            var locations = _databaseLocator.GetLocatesOrEmpty();

            foreach (var locate in locations)
            {
                var information = locate.Value;
                var collection = _client.GetDatabase(information.Database)
                    .GetCollection<BsonDocument>(information.Collection);

                var documents = collection.FindSync(_ => true).ToList();

                var type = locate.Key;

                var bulkOps = new List<WriteModel<BsonDocument>>();

                documents.ForEach(document =>
                {
                    Run(type, document);
                    var update = new ReplaceOneModel<BsonDocument>(
                        new BsonDocument {{"_id", document["_id"]}},
                        document
                    );
                    
                    bulkOps.Add(update);
                });

                collection.BulkWrite(bulkOps);
            }
        }
    }
}