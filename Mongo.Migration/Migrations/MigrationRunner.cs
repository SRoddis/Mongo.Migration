using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal class MigrationRunner : IMigrationRunner
    {
        private readonly IMigrationLocator _migrationLocator;

        private readonly IVersionService _versionService;

        public MigrationRunner(IMigrationLocator migrationLocator, IVersionService versionService)
        {
            _migrationLocator = migrationLocator;
            _versionService = versionService;
        }

        public void Run(Type type, BsonDocument document, DocumentVersion version)
        {
            var currentVersion = _versionService.GetVersion(type);

            if (version == currentVersion)
                return;

            if (version > currentVersion)
            {
                MigrateDown(type, document, currentVersion);
                return;
            }

            MigrateUp(type, document, version);
        }
        
        public void Run(Type type, BsonDocument document)
        {
            var documentVersion = _versionService.GetVersionOrDefault(document);
            
            Run(type, document, documentVersion);
        }

        private void MigrateUp(Type type, BsonDocument document, DocumentVersion version)
        {
            var migrations = _migrationLocator.GetMigrationsGt(type, version).ToList();

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