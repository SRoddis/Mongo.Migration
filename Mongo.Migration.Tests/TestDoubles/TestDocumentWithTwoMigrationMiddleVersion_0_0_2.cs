using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

public class TestDocumentWithTwoMigrationMiddleVersion002 : DocumentMigration<TestDocumentWithTwoMigrationMiddleVersion>
{
    public TestDocumentWithTwoMigrationMiddleVersion002()
        : base("0.0.2")
    {
    }

    public override void Up(BsonDocument document)
    {
        var doors = document["Doors1"].ToInt32();
        document.Add("Doors2", doors);
        document.Remove("Doors1");
    }

    public override void Down(BsonDocument document)
    {
        var doors = document["Doors2"].ToInt32();
        document.Add("Doors1", doors);
        document.Remove("Doors2");
    }
}