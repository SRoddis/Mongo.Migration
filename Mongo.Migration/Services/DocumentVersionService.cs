using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup;

using MongoDB.Bson;

namespace Mongo.Migration.Services;

internal sealed class DocumentVersionService : IDocumentVersionService
{
    private const string DefaultVersionFieldName = "Version";
    

    private readonly IMigrationLocator<IDocumentMigration> _migrationLocator;

    private readonly IRuntimeVersionLocator _runtimeVersionLocator;

    private readonly IStartUpVersionLocator _startUpVersionLocator;

    private readonly string _versionFieldName;

    public DocumentVersionService(
        IMigrationLocator<IDocumentMigration> migrationLocator,
        IRuntimeVersionLocator runtimeVersionLocator,
        IStartUpVersionLocator startUpVersionLocator,
        MongoMigrationSettings mongoMigrationSettings)
    {
        _migrationLocator = migrationLocator;
        _runtimeVersionLocator = runtimeVersionLocator;
        _startUpVersionLocator = startUpVersionLocator;
        _versionFieldName = string.IsNullOrWhiteSpace(mongoMigrationSettings.VersionFieldName)
            ? DefaultVersionFieldName
            : mongoMigrationSettings.VersionFieldName;
    }

    public string GetVersionFieldName()
    {
        return _versionFieldName;
    }

    public DocumentVersion GetCurrentOrLatestMigrationVersion(Type type)
    {
        var latestVersion = _migrationLocator.GetLatestVersion(type);
        return GetCurrentVersion(type) ?? latestVersion;
    }

    public DocumentVersion GetCollectionVersion(Type type)
    {
        var version = GetCurrentOrLatestMigrationVersion(type);
        return _startUpVersionLocator.GetLocateOrNull(type) ?? version;
    }

    public DocumentVersion GetVersionOrDefault(BsonDocument document)
    {
        if (document.TryGetValue(GetVersionFieldName(), out BsonValue value) && !value.IsBsonNull)
        {
            return DocumentVersion.Parse(value.AsString.AsSpan());
        }

        return DocumentVersion.Default;
    }

    public void SetVersion(BsonDocument document, in DocumentVersion version)
    {
        document[GetVersionFieldName()] = new BsonString(version.ToString());
    }

    public void DetermineVersion<TClass>(TClass instance)
        where TClass : IDocument
    {
        var type = typeof(TClass);
        var documentVersion = instance.Version;
        var latestVersion = _migrationLocator.GetLatestVersion(type);
        var currentVersion = GetCurrentVersion(type) ?? latestVersion;

        if (documentVersion == currentVersion)
        {
            return;
        }

        if (documentVersion == latestVersion)
        {
            return;
        }

        if (DocumentVersion.Default == documentVersion || DocumentVersion.Empty == documentVersion)
        {
            SetVersion(instance, currentVersion, latestVersion);
            return;
        }

        throw new VersionViolationException(currentVersion, documentVersion, latestVersion);
    }

    private DocumentVersion? GetCurrentVersion(Type type)
    {
        return _runtimeVersionLocator.GetLocateOrNull(type);
    }

    private static void SetVersion<TClass>(
        TClass instance,
        in DocumentVersion currentVersion,
        in DocumentVersion latestVersion)
        where TClass : IDocument
    {
        if (currentVersion < latestVersion)
        {
            instance.Version = currentVersion;
            return;
        }

        instance.Version = latestVersion;
    }
}