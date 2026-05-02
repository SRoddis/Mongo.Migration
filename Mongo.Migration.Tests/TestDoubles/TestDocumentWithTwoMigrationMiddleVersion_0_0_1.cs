using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

public class TestDocumentWithTwoMigrationMiddleVersion001 : DocumentMigration<TestDocumentWithTwoMigrationMiddleVersion>
{
    public TestDocumentWithTwoMigrationMiddleVersion001()
        : base("0.0.1")
    {
    }

    public override void Up(BsonDocument document)
    {
        var doors = document["Doors0"].ToInt32();
        document.Add("Doors1", doors);
        document.Remove("Doors0");
    }

    public override void Down(BsonDocument document)
    {
        var doors = document["Doors1"].ToInt32();
        document.Add("Doors0", doors);
        document.Remove("Doors1");
    }
}