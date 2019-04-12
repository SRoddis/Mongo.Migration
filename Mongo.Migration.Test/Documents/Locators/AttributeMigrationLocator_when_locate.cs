using System;
using FluentAssertions;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Documents.Locators
{
    [TestFixture]
    internal class VersionLocator_when_locate
    {
        [Test]
        public void Then_find_current_version_of_document()
        {
            // Arrange
            var locator = new VersionLocator();

            // Act
            var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithOneMigration));

            // Assert
            currentVersion.ToString().Should().Be("0.0.1");
        }

        [Test]
        public void When_document_has_no_attribute_Then_return_null()
        {
            // Arrange
            var locator = new VersionLocator();

            // Act
            var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithoutAttribute));

            // Assert
            currentVersion.Should().BeNull();
        }
    }
}