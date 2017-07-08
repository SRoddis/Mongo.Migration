using System;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal class MigrationRunner : IMigrationRunner
    {
        private const string VERSION_FIELD = "Version";

        private readonly IMigrationLocator _migrationLocator;

        private readonly IVersionLocator _versionLocator;

        public MigrationRunner(IMigrationLocator migrationLocator, IVersionLocator versionLocator)
        {
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
        }

        public void CheckVersion<TClass>(TClass instance) where TClass : class, IDocument
        {
            var type = typeof(TClass);
            var documentVersion = instance.Version.ToString();
            var currentVersion = _versionLocator.GetCurrentVersion(type);
            var latestVersion = _migrationLocator.GetLatestVersion(type);

            if (documentVersion == currentVersion)
                return;

            if (latestVersion == documentVersion)
                return;

            if (DocumentVersion.Default() == documentVersion)
            {
                instance.Version = latestVersion;
                return;
            }

            throw new VersionViolationException();
        }

        public void Run(Type type, BsonDocument document)
        {
            var documentVersion = GetVersionOrDefault(document);
            var currentVersion = _versionLocator.GetCurrentVersion(type);

            if (documentVersion == currentVersion)
                return;

            if (documentVersion > currentVersion)
                MigrateDown(type, currentVersion.ToString(), document);

            MigrateUp(type, documentVersion, document);
        }

        private void MigrateUp(Type type, DocumentVersion version, BsonDocument document)
        {
            var migrations = _migrationLocator.GetMigrationsGt(type, version).ToList();

            foreach (var migration in migrations)
            {
                migration.Up(document);
                SetVersion(document, migration.Version);
            }
        }

        private void MigrateDown(Type type, DocumentVersion version, BsonDocument document)
        {
            var migrations = _migrationLocator.GetMigrationsGtAndEquel(type, version).ToList();

            for (var m = 0; m < migrations.Count; m++)
            {
                if (version == migrations[m].Version)
                    break;

                migrations[m].Down(document);
                SetVersion(document, migrations[m + 1].Version);
            }
        }

        private static string GetVersionOrDefault(BsonDocument document)
        {
            BsonValue value;
            document.TryGetValue(VERSION_FIELD, out value);

            if (value != null)
                return value.AsString;

            return DocumentVersion.Default();
        }

        private static void SetVersion(BsonDocument document, string version)
        {
            document[VERSION_FIELD] = version;
        }
    }
}