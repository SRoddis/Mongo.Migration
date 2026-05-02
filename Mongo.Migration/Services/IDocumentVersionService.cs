using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Services;

public interface IDocumentVersionService
{
    string GetVersionFieldName();

    DocumentVersion GetCurrentOrLatestMigrationVersion(Type type);

    DocumentVersion GetCollectionVersion(Type type);

    DocumentVersion GetVersionOrDefault(BsonDocument document);

    void SetVersion(BsonDocument document, in DocumentVersion version);

    void DetermineVersion<TClass>(TClass instance)
        where TClass : IDocument;
}