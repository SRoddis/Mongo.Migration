using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document
{
    public interface IDocumentMigration : IMigration
    {
        void Up(BsonDocument document);
        void Down(BsonDocument document);
    }
}