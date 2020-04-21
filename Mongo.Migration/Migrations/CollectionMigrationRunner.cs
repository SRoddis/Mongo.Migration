using System;
using System.Collections.Generic;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class CollectionMigrationRunner : ICollectionMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly ICollectionLocator _collectionLocator;

        private readonly string[] _databaseNames;

        private readonly IMigrationRunner _migrationRunner;

        private readonly IVersionService _versionService;

        public CollectionMigrationRunner(
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IVersionService versionService,
            IMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                versionService,
                migrationRunner)
        {
            if (settings.ConnectionString == null && settings.Databases == null || settings.ClientSettings == null)
                throw new MongoMigrationNoMongoClientException();

            if (settings.ClientSettings != null)
                _client = new MongoClient(settings.ClientSettings);
            else
                _client = new MongoClient(settings.ConnectionString);

            _databaseNames = settings.Databases;
        }

        public CollectionMigrationRunner(
            IMongoClient client,
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IVersionService versionService,
            IMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                versionService,
                migrationRunner)
        {
            _client = client;

            if (settings.ConnectionString == null && settings.Databases == null) return;

            _client = new MongoClient(settings.ConnectionString);
            _databaseNames = settings.Databases;
        }

        private CollectionMigrationRunner(
            ICollectionLocator collectionLocator,
            IVersionService versionService,
            IMigrationRunner migrationRunner)
        {
            _collectionLocator = collectionLocator;
            _versionService = versionService;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var locations = _collectionLocator.GetLocatesOrEmpty();

            foreach (var locate in locations)
            {
                foreach (var dbName in _databaseNames)
                {
                    var information = locate.Value;
                    var type = locate.Key;
                    var databaseName = GetDatabaseOrDefault(information, dbName);
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
                                    new BsonDocument { { "_id", document["_id"] } },
                                    document
                                );

                                bulk.Add(update);
                            }
                        }
                    }

                    if (bulk.Count > 0) collection.BulkWrite(bulk);
                }
            }
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information, string dbName)
        {
            if (string.IsNullOrEmpty(dbName) && string.IsNullOrEmpty(information.Database))
                throw new NoDatabaseNameFoundException();

            return string.IsNullOrEmpty(information.Database) ? dbName : information.Database;
        }

        private FilterDefinition<BsonDocument> CreateQueryForRelevantDocuments(
            Type type)
        {
            var currentVersion = _versionService.GetCurrentOrLatestMigrationVersion(type);

            var existFilter = Builders<BsonDocument>.Filter.Exists(_versionService.GetVersionFieldName(), false);
            var notEqualFilter = Builders<BsonDocument>.Filter.Ne(
                _versionService.GetVersionFieldName(),
                currentVersion);

            return Builders<BsonDocument>.Filter.Or(existFilter, notEqualFilter);
        }
    }
}