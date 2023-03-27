using FluentAssertions;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Documents.Locators
{
    [TestFixture]
    internal class VersionLocatorWhenLocateTest
    {
        [Test]
        public void Then_find_current_version_of_document()
        {
            var locator = new RuntimeVersionLocator(new MongoMigrationAssemblyService(new MongoMigrationSettings()));

            var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithOneMigration));

            currentVersion.ToString().Should().Be("0.0.1");
        }

        [Test]
        public void When_document_has_no_attribute_Then_return_null()
        {
            var locator = new RuntimeVersionLocator(new MongoMigrationAssemblyService(new MongoMigrationSettings()));

            var currentVersion = locator.GetLocateOrNull(typeof(TestDocumentWithoutAttribute));

            currentVersion.Should().BeNull();
        }
    }
}