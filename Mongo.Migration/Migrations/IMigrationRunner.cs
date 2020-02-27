using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal interface IMigrationRunner
    {
        IMigrationLocator _migrationLocator { get; }
        
        void Run(Type type, BsonDocument document, DocumentVersion to);
        
        void Run(Type type, BsonDocument document);
    }
}