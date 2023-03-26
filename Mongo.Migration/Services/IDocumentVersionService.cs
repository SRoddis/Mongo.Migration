using System;
using System.Collections.Generic;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Services
{
    public interface IDocumentVersionService
    {
        string GetVersionFieldName();
        DocumentVersion GetCurrentOrLatestMigrationVersion(Type type);
        DocumentVersion GetCollectionVersion(Type type);
        DocumentVersion GetVersionOrDefault(BsonDocument document);
        void SetVersion(BsonDocument document, DocumentVersion version);
        void DetermineVersion<TClass>(TClass instance)
            where TClass : class, IDocument;
        DocumentVersion DetermineLastVersion(
            DocumentVersion version,
            List<IDocumentMigration> migrations,
            int currentMigration);
    }
}