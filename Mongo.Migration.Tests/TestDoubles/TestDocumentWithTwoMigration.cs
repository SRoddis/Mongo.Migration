using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

[RuntimeVersion("0.0.0")]
public class TestDocumentWithTwoMigration : Document
{
    public ObjectId Id { get; set; }
    public int Dors { get; set; }
}