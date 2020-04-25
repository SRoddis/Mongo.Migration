using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    public interface IDocumentMigration : IMigration
    {
        void Up(BsonDocument document);

        void Down(BsonDocument document);
    }
}