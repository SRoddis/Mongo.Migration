using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents;

[TestFixture]
public class DocumentWhenCreating
{
    [Test]
    public void Then_document_can_be_created()
    {
        // Arrange Act
        Document document = new();

        // Assert
        Assert.That(document, Is.TypeOf<Document>());
    }

    [Test]
    public void Then_document_has_a_version()
    {
        // Arrange 
        Document document = new();

        // Act
        var version = document.Version;

        // Assert
        Assert.That(version.ToString(), Is.EqualTo("0.0.0"));
    }
}