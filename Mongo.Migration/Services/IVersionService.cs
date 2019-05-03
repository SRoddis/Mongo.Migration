using System;
using System.Collections.Generic;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations;
using MongoDB.Bson;

namespace Mongo.Migration.Services
{
    public interface IVersionService
    {
        string GetVersionFieldName();

        DocumentVersion GetVersion(Type type);

        DocumentVersion GetVersionOrDefault(BsonDocument document);

        void SetVersion(BsonDocument document, DocumentVersion version);

        void DetermineVersion<TClass>(TClass instance) where TClass : class, IDocument;

        DocumentVersion DetermineLastVersion(
            DocumentVersion version,
            List<IMigration> migrations,
            int currentMigration);
    }
}