using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

[RuntimeVersion("0.0.1")]
public class TestDocumentWithTwoMigrationMiddleVersion : Document
{
    public ObjectId Id { get; set; }
    public int Doors1 { get; set; }
}