using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    public interface IAdvancedMigration
    {
        string Version { get; }

        string CollectionName { get; }

        Type Type { get; }

        void Up(IMongoDatabase db);

        void Down(IMongoDatabase db);

        void Up(BsonDocument document);

        void Down(BsonDocument document);
    }
}