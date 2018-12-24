using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal interface IMigrationRunner<TBaseDocument>
    {
        void Run(Type type, BsonDocument document);

        void CheckVersion<TClass>(TClass instance) where TClass : class, TBaseDocument;
    }
}