using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal class MigrationRunner : IMigrationRunner
    {
        private const string VERSION_FIELD = "Version";

        private readonly IMigrationLocator _migrationLocator;

        public MigrationRunner(IMigrationLocator migrationLocator)
        {
            _migrationLocator = migrationLocator;
        }

        public void CheckVersion<TClass>(TClass instance) where TClass : class, IDocument
        {
            var type = typeof(TClass);
            var currentVersion = instance.Version.ToString();
            var latestVersion = _migrationLocator.GetLatestVersion(type);

            if (latestVersion == currentVersion)
                return;

            if (DocumentVersion.Default() == currentVersion)
            {
                instance.Version = latestVersion;
                return;
            }

            throw new VersionViolationException();
        }

        public void Run(Type type, BsonDocument document)
        {
            var currentVersion = GetVersionOrDefault(document);
            var migrations = _migrationLocator.GetMigrationsGt(type, currentVersion);

            foreach (var migration in migrations)
            {
                migration.Up(document);
                SetVersion(document, migration.Version);
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