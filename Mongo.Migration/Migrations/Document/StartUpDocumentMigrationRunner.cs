using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Document;

internal class StartUpDocumentMigrationRunner : IStartUpDocumentMigrationRunner
{
    private readonly ICollectionLocator _collectionLocator;

    private readonly IDocumentVersionService _documentVersionService;

    private readonly IDocumentMigrationRunner _migrationRunner;

    public StartUpDocumentMigrationRunner(
        ICollectionLocator collectionLocator,
        IDocumentVersionService documentVersionService,
        IDocumentMigrationRunner migrationRunner)
    {
        _collectionLocator = collectionLocator;
        _documentVersionService = documentVersionService;
        _migrationRunner = migrationRunner;
    }

    public async Task RunAllAsync(IMongoDatabase database, CancellationToken cancellationToken)
    {
        var locations = _collectionLocator.GetLocatesOrEmpty();

        foreach (var locate in locations)
        {
            var information = locate.Value;
            var type = locate.Key;
            var collectionVersion = _documentVersionService.GetCollectionVersion(type);

            var collection = database.GetCollection<BsonDocument>(information.Collection);

            var bulk = new List<WriteModel<BsonDocument>>();

            var query = CreateQueryForRelevantDocuments(type);

            using (var cursor = await collection.FindAsync(query, cancellationToken: cancellationToken))
            {
                while (await cursor.MoveNextAsync(cancellationToken))
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
                await collection.BulkWriteAsync(bulk, cancellationToken: cancellationToken);
            }
        }
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