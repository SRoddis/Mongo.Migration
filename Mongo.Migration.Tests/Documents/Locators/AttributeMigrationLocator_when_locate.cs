using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents.Locators;

[TestFixture]
internal class VersionLocatorWhenLocate
{
    [Test]
    public void Then_find_current_version_of_document()
    {
        // Arrange
        var locator = new RuntimeVersionLocator([]);

        // Act
        var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithOneMigration));

        // Assert
        Assert.That(currentVersion.ToString(), Is.EqualTo("0.0.1"));
    }

    [Test]
    public void When_document_has_no_attribute_Then_return_null()
    {
        // Arrange
        var locator = new RuntimeVersionLocator([]);

        // Act
        var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithoutAttribute));

        // Assert
        Assert.That(currentVersion, Is.Null);
    }
}