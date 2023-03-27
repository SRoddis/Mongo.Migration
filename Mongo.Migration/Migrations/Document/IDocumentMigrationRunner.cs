using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document
{
    internal interface IDocumentMigrationRunner
    {
        void Run(Type type, BsonDocument document, DocumentVersion to);
        void Run(Type type, BsonDocument document);
    }
}