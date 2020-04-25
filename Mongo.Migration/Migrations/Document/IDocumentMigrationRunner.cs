using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal interface IMigrationRunner
    {
        void Run(Type type, BsonDocument document, DocumentVersion to);
        
        void Run(Type type, BsonDocument document);
    }
}