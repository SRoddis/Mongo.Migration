using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Document;

[TestFixture]
public class DocumentMigrationWhenMigrating
{
    [Test]
    public void When_migrating_down_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration001();
        var document = new BsonDocument { { "Doors", 3 } };

        // Act
        migration.Down(document);

        // Assert
        Assert.That(document, Is.EquivalentTo(new BsonDocument { { "Dors", 3 } }));
    }

    [Test]
    public void When_migrating_up_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration001();
        var document = new BsonDocument { { "Dors", 3 } };

        // Act
        migration.Up(document);

        // Assert
        Assert.That(document, Is.EquivalentTo(new BsonDocument { { "Doors", 3 } }));
    }

    [Test]
    public void When_version_as_string_migrating_down_Then_document_changes()
    {
        // Arrange
        var migration = new TestClassWithTwoMigrationMiddleVersion001();
        var document = new BsonDocument { { "Doors1", 3 } };

        // Act
        migration.Down(document);

        // Assert
        Assert.That(document, Is.EquivalentTo(new BsonDocument { { "Doors0", 3 } }));
    }

    [Test]
    public void When_version_as_string_migrating_up_Then_document_changes()
    {
        // Arrange
        var migration = new TestClassWithTwoMigrationMiddleVersion001();
        var document = new BsonDocument { { "Doors0", 3 } };

        // Act
        migration.Up(document);

        // Assert
        Assert.That(document, Is.EquivalentTo(new BsonDocument { { "Doors1", 3 } }));
    }
}