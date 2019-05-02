using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class CollectionMigrationRunner : ICollectionMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly ICollectionLocator _collectionLocator;

        private readonly string _databaseName;

        private readonly IMigrationRunner _migrationRunner;

        public CollectionMigrationRunner(IOptions<MongoMigrationSettings> options, ICollectionLocator collectionLocator,
            IMigrationRunner migrationRunner)
            : this(new MongoClient(options.Value.ConnectionString), collectionLocator, migrationRunner)
        {
            _databaseName = options.Value.Database;
            _collectionLocator = collectionLocator;
        }

        public CollectionMigrationRunner(IMongoClient client, ICollectionLocator collectionLocator,
            IMigrationRunner migrationRunner)
        {
            _client = client;
            _collectionLocator = collectionLocator;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var locations = _collectionLocator.GetLocatesOrEmpty();

            foreach (var locate in locations)
            {
                var information = locate.Value;
                var databaseName = GetDatabaseOrDefault(information);

                var collection = _client.GetDatabase(databaseName)
                    .GetCollection<BsonDocument>(information.Collection);

                var type = locate.Key;

                var bulkOps = new List<WriteModel<BsonDocument>>();

                // where version != current || version does not exist!!
/*                var existFilter = Builders<BsonDocument>.Filter.Exists(MigrationRunner.VERSION_FIELD, false);
                var existFilter = Builders<BsonDocument>.Filter.Exists(MigrationRunner.VERSION_FIELD, false);*/
                using (var cursor = collection.FindSync(_ => true))
                {
                    while (cursor.MoveNext())
                    {
                        var batch = cursor.Current;
                        foreach (BsonDocument document in batch)
                        {
                            _migrationRunner.Run(type, document);
                            var update = new ReplaceOneModel<BsonDocument>(
                                new BsonDocument {{"_id", document["_id"]}},
                                document
                            );

                            bulkOps.Add(update);
                        }
                    }
                }

                collection.BulkWrite(bulkOps);
            }
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information)
        {
            if (string.IsNullOrEmpty(_databaseName) && string.IsNullOrEmpty(information.Database))
                throw new NoDatabaseNameFoundException();

            return string.IsNullOrEmpty(information.Database) ? _databaseName : information.Database;
        }
    }
}