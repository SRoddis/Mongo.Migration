using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class CollectionMigrationRunner : ICollectionMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly ICollectionLocator _collectionLocator;
        
        private readonly IMigrationRunner _migrationRunner;

        private readonly string _databaseName;

        public CollectionMigrationRunner(IOptions<MongoMigrationSettings> options, ICollectionLocator collectionLocator, IMigrationRunner migrationRunner)
            : this(new MongoClient(options.Value.ConnectionString), collectionLocator, migrationRunner)
        {
            _databaseName = options.Value.Database;
            _collectionLocator = collectionLocator;
        }
        
        public CollectionMigrationRunner(IMongoClient client, ICollectionLocator collectionLocator, IMigrationRunner migrationRunner)
        {
            _client = client;
            _collectionLocator = collectionLocator;
            _migrationRunner = migrationRunner;
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information)
        {
            if (String.IsNullOrEmpty(information.Database))
            {
                return _databaseName;
            }

            return information.Database;
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

                var documents = collection.FindSync(_ => true).ToList();

                var type = locate.Key;

                var bulkOps = new List<WriteModel<BsonDocument>>();

                documents.ForEach(document =>
                {
                    _migrationRunner.Run(type, document);
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