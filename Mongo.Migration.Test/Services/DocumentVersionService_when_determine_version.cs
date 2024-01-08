using FluentAssertions;

using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Test.TestDoubles;

using NUnit.Framework;

namespace Mongo.Migration.Test.Services;

[TestFixture]
internal class DocumentVersionService_when_determine_version : IntegrationTest
{
    private IDocumentVersionService _service;

    [SetUp]
    public void SetUp()
    {
        this.OnSetUp();

        this._service = this._components.Get<IDocumentVersionService>();
    }

    [TearDown]
    public void TearDown()
    {
        this.Dispose();
    }

    [Test]
    public void When_document_has_current_version_Then_current_version_is_set()
    {
        // Arrange
        var document = new TestDocumentWithTwoMigrationMiddleVersion();

        // Act
        this._service.DetermineVersion(document);

        // Assert
        document.Version.Should().Be("0.0.1");
    }

    [Test]
    public void When_document_has_highest_version_Then_highest_version_is_set()
    {
        // Arrange
        var document = new TestDocumentWithTwoMigrationHighestVersion();

        // Act
        this._service.DetermineVersion(document);

        // Assert
        document.Version.Should().Be("0.0.2");
    }

    [Test]
    public void When_document_has_version_that_should_not_be_Then_throw_exception()
    {
        // Arrange
        var document = new TestDocumentWithTwoMigrationHighestVersion { Version = "0.0.1" };

        // Act// Act
        var checkAction = () => { this._service.DetermineVersion(document); };

        // Assert
        checkAction.Should().Throw<VersionViolationException>();
    }
}