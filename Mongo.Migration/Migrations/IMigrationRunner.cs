using System;
using Mongo.Migration.Models;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    internal interface IMigrationRunner
    {
        void Run(Type type, BsonDocument document);

        void CheckVersion<TClass>(TClass instance) where TClass : class, IDocument;
    }
}