using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document;

internal class DocumentMigrationRunner : IDocumentMigrationRunner
{
    private readonly IDocumentVersionService _documentVersionService;

    private readonly IMigrationLocator<IDocumentMigration> _migrationLocator;

    public DocumentMigrationRunner(IMigrationLocator<IDocumentMigration> migrationLocator, IDocumentVersionService documentVersionService)
    {
        _migrationLocator = migrationLocator;
        _documentVersionService = documentVersionService;
    }

    public void Run(Type type, BsonDocument document)
    {
        var currentOrLatest = _documentVersionService.GetCurrentOrLatestMigrationVersion(type);
        Run(type, document, currentOrLatest);
    }

    public void Run(Type type, BsonDocument document, in DocumentVersion to)
    {
        var documentVersion = _documentVersionService.GetVersionOrDefault(document);

        if (documentVersion == to)
        {
            return;
        }

        MigrateUpOrDown(type, document, documentVersion, to);
    }

    private void MigrateUpOrDown(
        Type type,
        BsonDocument document,
        in DocumentVersion documentVersion,
        in DocumentVersion to)
    {
        if (documentVersion > to)
        {
            MigrateDown(type, document, documentVersion, to);
        }
        else
        {
            MigrateUp(type, document, documentVersion, to);
        }
        
        _documentVersionService.SetVersion(document, to);
    }

    private void MigrateUp(Type type, BsonDocument document, in DocumentVersion version, in DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromTo(type, version, toVersion);

        foreach (var migration in migrations)
        {
            migration.Up(document);
        }
    }

    private void MigrateDown(Type type, BsonDocument document, in DocumentVersion version, in DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
                .GetMigrationsFromToDown(type, version, toVersion);

        foreach (var migration in migrations)
        {
            migration.Down(document);
        }
    }
}