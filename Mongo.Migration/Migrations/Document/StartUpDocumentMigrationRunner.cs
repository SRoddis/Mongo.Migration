using System;
using System.Collections.Generic;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Document
{
    internal class StartUpDocumentMigrationRunner : IStartUpDocumentMigrationRunner
    {
        private readonly IMongoClient _client;
        private readonly ICollectionLocator _collectionLocator;
        private readonly string _databaseName;
        private readonly IDocumentVersionService _documentVersionService;
        private readonly IDocumentMigrationRunner _migrationRunner;

        public StartUpDocumentMigrationRunner(
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IDocumentVersionService documentVersionService,
            IDocumentMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                documentVersionService,
                migrationRunner)
        {
            if (settings.ConnectionString == null && settings.Database == null || settings.ClientSettings == null)
            {
                throw new MongoMigrationNoMongoClientException();
            }

            _client = settings.ClientSettings != null ? new MongoClient(settings.ClientSettings) : new MongoClient(settings.ConnectionString);

            _databaseName = settings.Database;
        }

        public StartUpDocumentMigrationRunner(
            IMongoClient client,
            IMongoMigrationSettings settings,
            ICollectionLocator collectionLocator,
            IDocumentVersionService documentVersionService,
            IDocumentMigrationRunner migrationRunner)
            : this(
                collectionLocator,
                documentVersionService,
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

        private StartUpDocumentMigrationRunner(
            ICollectionLocator collectionLocator,
            IDocumentVersionService documentVersionService,
            IDocumentMigrationRunner migrationRunner)
        {
            _collectionLocator = collectionLocator;
            _documentVersionService = documentVersionService;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var locations = _collectionLocator.GetLocatesOrEmpty();

            foreach (var (type, information) in locations)
            {
                var databaseName = GetDatabaseOrDefault(information);
                var collectionVersion = _documentVersionService.GetCollectionVersion(type);

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

                if (bulk.Count > 0)
                {
                    collection.BulkWrite(bulk);
                }
            }
        }

        private string GetDatabaseOrDefault(CollectionLocationInformation information)
        {
            if (string.IsNullOrEmpty(_databaseName) && string.IsNullOrEmpty(information.Database))
            {
                throw new NoDatabaseNameFoundException();
            }

            return string.IsNullOrEmpty(information.Database) ? _databaseName : information.Database;
        }

        private FilterDefinition<BsonDocument> CreateQueryForRelevantDocuments(Type type)
        {
            var currentVersion = _documentVersionService.GetCurrentOrLatestMigrationVersion(type);

            var existFilter = Builders<BsonDocument>.Filter.Exists(_documentVersionService.GetVersionFieldName(), false);
            var notEqualFilter = Builders<BsonDocument>.Filter.Ne(
                _documentVersionService.GetVersionFieldName(),
                currentVersion);

            return Builders<BsonDocument>.Filter.Or(existFilter, notEqualFilter);
        }
    }
}