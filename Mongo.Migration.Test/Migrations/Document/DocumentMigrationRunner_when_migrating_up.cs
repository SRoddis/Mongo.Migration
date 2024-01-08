using System.Linq;

using FluentAssertions;

using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Test.TestDoubles;

using MongoDB.Bson;

using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Document;

[TestFixture]
internal class DocumentMigrationRunner_when_migrating_up : IntegrationTest
{
    private IDocumentMigrationRunner _runner;

    [SetUp]
    public void SetUp()
    {
        this.OnSetUp();

        this._runner = this._components.Get<IDocumentMigrationRunner>();
    }

    [TearDown]
    public void TearDown()
    {
        this.Dispose();
    }

    [Test]
    public void When_migrate_up_the_lowest_version_Then_all_migrations_are_used()
    {
        // Arrange
        var document = new BsonDocument
        {
            { "Version", "0.0.0" },
            { "Dors", 3 }
        };

        // Act
        this._runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

        // Assert
        document.Names.ToList()[1].Should().Be("Door");
        document.Values.ToList()[0].AsString.Should().Be("0.0.2");
    }

    [Test]
    public void When_document_has_no_version_Then_all_migrations_are_used()
    {
        // Arrange
        var document = new BsonDocument
        {
            { "Dors", 3 }
        };

        // Act
        this._runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

        // Assert
        document.Names.ToList()[1].Should().Be("Door");
        document.Values.ToList()[0].AsString.Should().Be("0.0.2");
    }

    [Test]
    public void When_document_has_current_version_Then_nothing_happens()
    {
        // Arrange
        var document = new BsonDocument
        {
            { "Version", "0.0.2" },
            { "Door", 3 }
        };

        // Act
        this._runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

        // Assert
        document.Names.ToList()[1].Should().Be("Door");
        document.Values.ToList()[0].AsString.Should().Be("0.0.2");
    }
}