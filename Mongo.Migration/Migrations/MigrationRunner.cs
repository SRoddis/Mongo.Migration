using System;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal class MigrationRunner : IMigrationRunner
    {
        private readonly IMigrationLocator<IDocumentMigration> _migrationLocator;

        private readonly IVersionService _versionService;

        public MigrationRunner(IMigrationLocator<IDocumentMigration> migrationLocator, IVersionService versionService)
        {
            _migrationLocator = migrationLocator;
            _versionService = versionService;
        }
        
        public void Run(Type type, BsonDocument document)
        {
            var documentVersion = _versionService.GetVersionOrDefault(document);
            var currentOrLatest = _versionService.GetCurrentOrLatestMigrationVersion(type);

            if (documentVersion == currentOrLatest)
                return;

            MigrateUpOrDown(type, document, documentVersion, currentOrLatest);
        }
        
        public void Run(Type type, BsonDocument document, DocumentVersion to)
        {
            var documentVersion = _versionService.GetVersionOrDefault(document);
            var currentOrLatest = _versionService.GetCurrentOrLatestMigrationVersion(type);

            if (documentVersion == to || documentVersion == currentOrLatest)
                return;
            
            MigrateUpOrDown(type, document, documentVersion, to);
        }

        private void MigrateUpOrDown(
            Type type,
            BsonDocument document,
            DocumentVersion documentVersion,
            DocumentVersion to)
        {
            if (documentVersion > to)
            {
                MigrateDown(type, document, to);
                return;
            }

            MigrateUp(type, document, documentVersion, to);
        }

        private void MigrateUp(Type type, BsonDocument document, DocumentVersion version, DocumentVersion toVersion)
        {
            var migrations = _migrationLocator.GetMigrationsFromTo(type, version, toVersion).ToList();
            
            foreach (var migration in migrations)
            {
                migration.Up(document);
                _versionService.SetVersion(document, migration.Version);
            }
        }

        private void MigrateDown(Type type, BsonDocument document, DocumentVersion version)
        {
            var migrations = _migrationLocator
                .GetMigrationsGtEq(type, version)
                .OrderByDescending(m => m.Version)
                .ToList();

            for (var m = 0; m < migrations.Count; m++)
            {
                if (version == migrations[m].Version)
                    break;

                migrations[m].Down(document);

                var docVersion = _versionService.DetermineLastVersion(version, migrations, m);
                _versionService.SetVersion(document, docVersion);
            }
        }
    }
}