using System;
using System.Linq;

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
        this._migrationLocator = migrationLocator;
        this._documentVersionService = documentVersionService;
    }

    public void Run(Type type, BsonDocument document)
    {
        var documentVersion = this._documentVersionService.GetVersionOrDefault(document);
        var currentOrLatest = this._documentVersionService.GetCurrentOrLatestMigrationVersion(type);

        if (documentVersion == currentOrLatest)
        {
            return;
        }

        this.MigrateUpOrDown(type, document, documentVersion, currentOrLatest);
    }

    public void Run(Type type, BsonDocument document, DocumentVersion to)
    {
        var documentVersion = this._documentVersionService.GetVersionOrDefault(document);
        var currentOrLatest = this._documentVersionService.GetCurrentOrLatestMigrationVersion(type);

        if (documentVersion == to || documentVersion == currentOrLatest)
        {
            return;
        }

        this.MigrateUpOrDown(type, document, documentVersion, to);
    }

    private void MigrateUpOrDown(
        Type type,
        BsonDocument document,
        DocumentVersion documentVersion,
        DocumentVersion to)
    {
        if (documentVersion > to)
        {
            this.MigrateDown(type, document, to);
            return;
        }

        this.MigrateUp(type, document, documentVersion, to);
    }

    private void MigrateUp(Type type, BsonDocument document, DocumentVersion version, DocumentVersion toVersion)
    {
        var migrations = this._migrationLocator.GetMigrationsFromTo(type, version, toVersion).ToList();

        foreach (var migration in migrations)
        {
            migration.Up(document);
            this._documentVersionService.SetVersion(document, migration.Version);
        }
    }

    private void MigrateDown(Type type, BsonDocument document, DocumentVersion version)
    {
        var migrations = this._migrationLocator
            .GetMigrationsGtEq(type, version)
            .OrderByDescending(m => m.Version)
            .ToList();

        for (var m = 0; m < migrations.Count; m++)
        {
            if (version == migrations[m].Version)
            {
                break;
            }

            migrations[m].Down(document);

            var docVersion = this._documentVersionService.DetermineLastVersion(version, migrations, m);
            this._documentVersionService.SetVersion(document, docVersion);
        }
    }
}