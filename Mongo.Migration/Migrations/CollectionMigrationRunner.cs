using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class  CollectionMigrationRunner : ICollectionMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly ICollectionLocator _collectionLocator;

        private readonly IMigrationRunner _migrationRunner;

        private readonly IVersionService _versionService;
        
        private readonly string _databaseName;

        public CollectionMigrationRunner(
            IOptions<MongoMigrationSettings> options,
            ICollectionLocator collectionLocator,
            IVersionService versionService,
            IMigrationRunner migrationRunner)
            : this(
                new MongoClient(options.Value.ConnectionString),
                collectionLocator,
                versionService,
                migrationRunner)
        {
            _databaseName = options.Value.Database;
            _collectionLocator = collectionLocator;
        }

        public CollectionMigrationRunner(
            IMongoClient client,
            ICollectionLocator collectionLocator,
            IVersionService versionService,
            IMigrationRunner migrationRunner)
        {
            _client = client;
            _collectionLocator = collectionLocator;
            _versionService = versionService;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var locations = _collectionLocator.GetLocatesOrEmpty();

            foreach (var locate in locations)
            {
                var information = locate.Value;
                var type = locate.Key;
                var databaseName = GetDatabaseOrDefault(information);
                var collectionVersion = _versionService.GetCollectionVersion(type);

                var collection = _client.GetDatabase(databaseName)
                    .GetCollection<BsonDocument>(information.Collection);

                var bulk = new List<WriteModel<BsonDocument>>();

                var query = CreateQueryForRelevantDocuments(type);

                using (var cursor = collection.FindSync(query))
                {
                    while (cursor.MoveNext())
                    {
                        var batch = cursor.Current;
                        foreach (var document in batch)
                        {
                            _migrationRunner.Run(type, document, collectionVersion);
                            var update = new ReplaceOneModel<BsonDocument>(
                                new BsonDocument {{"_id", document["_id"]}},
                                document
                            );

                            bulk.Add(update);
                        }
                    }
                }

                if (bulk.Count > 0)
                {
                    collection.BulkWrite(bulk);
                }
            }
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information)
        {
            if (string.IsNullOrEmpty(_databaseName) && string.IsNullOrEmpty(information.Database))
                throw new NoDatabaseNameFoundException();

            return string.IsNullOrEmpty(information.Database) ? _databaseName : information.Database;
        }

        private FilterDefinition<BsonDocument> CreateQueryForRelevantDocuments(
            Type type)
        {
            var currentVersion = _versionService.GetVersion(type);

            var existFilter = Builders<BsonDocument>.Filter.Exists(_versionService.GetVersionFieldName(), false);
            var notEqualFilter = Builders<BsonDocument>.Filter.Ne(
                _versionService.GetVersionFieldName(),
                currentVersion);

            return Builders<BsonDocument>.Filter.Or(existFilter, notEqualFilter);
        }
    }
}