using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;

namespace Mongo.Migration.Services
{
    internal class VersionService : IVersionService
    {
        private static readonly string VERSION_FIELD_NAME = "Version";

        private readonly IMigrationLocator _migrationLocator;

        private readonly IVersionLocator _versionLocator;

        public VersionService(IMigrationLocator migrationLocator, IVersionLocator versionLocator)
        {
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
        }

        public string GetVersionFieldName()
        {
            return VERSION_FIELD_NAME;
        }

        public DocumentVersion GetVersion(Type type)
        {
            var latestVersion = _migrationLocator.GetLatestVersion(type);
            return _versionLocator.GetLocateOrNull(type) ?? latestVersion;
        }

        public DocumentVersion GetVersionOrDefault(BsonDocument document)
        {
            BsonValue value;
            document.TryGetValue(GetVersionFieldName(), out value);

            if (value != null)
                return value.AsString;

            return DocumentVersion.Default();
        }

        public void SetVersion(BsonDocument document, DocumentVersion version)
        {
            document[GetVersionFieldName()] = version.ToString();
        }

        public void DetermineVersion<TClass>(TClass instance) where TClass : class, IDocument
        {
            var type = typeof(TClass);
            var documentVersion = instance.Version.ToString();
            var latestVersion = _migrationLocator.GetLatestVersion(type);
            var currentVersion = _versionLocator.GetLocateOrNull(type) ?? latestVersion;

            if (documentVersion == currentVersion)
                return;

            if (documentVersion == latestVersion)
                return;

            if (DocumentVersion.Default() == documentVersion)
            {
                SetVersion(instance, currentVersion, latestVersion);
                return;
            }

            throw new VersionViolationException(currentVersion.ToString(), documentVersion, latestVersion);
        }

        public DocumentVersion DetermineLastVersion(DocumentVersion version, List<IMigration> migrations,
            int currentMigration)
        {
            if (migrations.Last() != migrations[currentMigration])
                return migrations[currentMigration + 1].Version;
            return version;
        }

        private static void SetVersion<TClass>(
            TClass instance,
            DocumentVersion? currentVersion,
            DocumentVersion latestVersion) where TClass : class, IDocument
        {
            if (currentVersion < latestVersion)
            {
                instance.Version = currentVersion.ToString();
                return;
            }

            instance.Version = latestVersion;
        }
    }
}