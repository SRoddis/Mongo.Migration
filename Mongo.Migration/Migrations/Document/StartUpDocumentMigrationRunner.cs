using System;
using System.Collections.Generic;

using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Document;

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
        if ((settings.ConnectionString == null && settings.Database == null) || settings.ClientSettings == null)
        {
            throw new MongoMigrationNoMongoClientException();
        }

        if (settings.ClientSettings != null)
        {
            this._client = new MongoClient(settings.ClientSettings);
        }
        else
        {
            this._client = new MongoClient(settings.ConnectionString);
        }

        this._databaseName = settings.Database;
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
        this._client = client;

        if (settings.ConnectionString == null && settings.Database == null)
        {
            return;
        }

        this._client = new MongoClient(settings.ConnectionString);
        this._databaseName = settings.Database;
    }

    private StartUpDocumentMigrationRunner(
        ICollectionLocator collectionLocator,
        IDocumentVersionService documentVersionService,
        IDocumentMigrationRunner migrationRunner)
    {
        this._collectionLocator = collectionLocator;
        this._documentVersionService = documentVersionService;
        this._migrationRunner = migrationRunner;
    }

    public void RunAll()
    {
        var locations = this._collectionLocator.GetLocatesOrEmpty();

        foreach (var locate in locations)
        {
            var information = locate.Value;
            var type = locate.Key;
            var databaseName = this.GetDatabaseOrDefault(information);
            var collectionVersion = this._documentVersionService.GetCollectionVersion(type);

            var collection = this._client.GetDatabase(databaseName)
                .GetCollection<BsonDocument>(information.Collection);

            var bulk = new List<WriteModel<BsonDocument>>();

            var query = this.CreateQueryForRelevantDocuments(type);

            using (var cursor = collection.FindSync(query))
            {
                while (cursor.MoveNext())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        this._migrationRunner.Run(type, document, collectionVersion);

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
        if (string.IsNullOrEmpty(this._databaseName) && string.IsNullOrEmpty(information.Database))
        {
            throw new NoDatabaseNameFoundException();
        }

        return string.IsNullOrEmpty(information.Database) ? this._databaseName : information.Database;
    }

    private FilterDefinition<BsonDocument> CreateQueryForRelevantDocuments(
        Type type)
    {
        var currentVersion = this._documentVersionService.GetCurrentOrLatestMigrationVersion(type);

        var existFilter = Builders<BsonDocument>.Filter.Exists(this._documentVersionService.GetVersionFieldName(), false);
        var notEqualFilter = Builders<BsonDocument>.Filter.Ne(
            this._documentVersionService.GetVersionFieldName(),
            currentVersion);

        return Builders<BsonDocument>.Filter.Or(existFilter, notEqualFilter);
    }
}