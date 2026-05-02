using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;
public class TestClassWithTwoMigrationMiddleVersion
{
    public ObjectId Id { get; set; }
    public int Doors1 { get; set; }
    public string Version { get; set; } = "0.0.0";
}

public class TestClassWithTwoMigrationMiddleVersion001 : DocumentMigration<TestClassWithTwoMigrationMiddleVersion>
{
    public TestClassWithTwoMigrationMiddleVersion001() : base("0.0.1") { }

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

public class TestClassWithTwoMigrationMiddleVersion002 : DocumentMigration<TestClassWithTwoMigrationMiddleVersion>
{
    public TestClassWithTwoMigrationMiddleVersion002() : base("0.0.2") { }

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