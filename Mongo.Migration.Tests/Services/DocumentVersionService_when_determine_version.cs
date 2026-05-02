using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services;

[TestFixture]
internal class DocumentVersionServiceWhenDetermineVersion : IntegrationTest
{
    [Test]
    public void When_document_has_current_version_Then_current_version_is_set()
    {
        // Arrange
        IDocumentVersionService service = TestcontainersContext.Provider.GetRequiredService<IDocumentVersionService>();
        var document = new TestDocumentWithTwoMigrationMiddleVersion();

        // Act
        service.DetermineVersion(document);

        // Assert
        Assert.That(document.Version.ToString(), Is.EqualTo("0.0.1"));
    }

    [Test]
    public void When_document_has_highest_version_Then_highest_version_is_set()
    {
        // Arrange
        IDocumentVersionService service = TestcontainersContext.Provider.GetRequiredService<IDocumentVersionService>();
        var document = new TestDocumentWithTwoMigrationHighestVersion();

        // Act
        service.DetermineVersion(document);

        // Assert
        Assert.That(document.Version.ToString(), Is.EqualTo("0.0.2"));
    }

    [Test]
    public void When_document_has_version_that_should_not_be_Then_throw_exception()
    {
        // Arrange
        IDocumentVersionService service = TestcontainersContext.Provider.GetRequiredService<IDocumentVersionService>();
        var document = new TestDocumentWithTwoMigrationHighestVersion { Version = new DocumentVersion(0,0,1) };

        // Assert
        Assert.Throws<VersionViolationException>(() => service.DetermineVersion(document));
    }
}