using System;
using System.Linq;
using FluentAssertions;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Locators
{
    [TestFixture]
    public class AttributeMigrationLocator_when_locate
    {
        private AttributeMigrationLocator _locator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Arrange
            _locator = new AttributeMigrationLocator();
        }

        [Test]
        public void When_document_has_one_migration_Then_migrations_count_should_be_one()
        {
            // Act
            var result = _locator.GetMigrations(typeof(TestDocumentWithOneMigration));

            // Assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void When_document_has_two_migration_Then_migrations_count_should_be_two()
        {
            // Act
            var result = _locator.GetMigrations(typeof(TestDocumentWithTwoMigration));

            // Assert
            result.Count().Should().Be(2);
        }

        [Test]
        public void When_get_latest_version_of_migrations()
        {
            // Act
            var version = _locator.GetLatestVersion(typeof(TestDocumentWithTwoMigration));

            // Assert
            version.Should().Be("0.0.2");
        }

        [Test]
        public void When_get_migrations_gt_and_equal_version()
        {
            // Act
            var result = _locator.GetMigrationsGtEq(typeof(TestDocumentWithTwoMigration), "0.0.1").ToList();

            // Assert
            result[0].Should().BeOfType<TestDocumentWithTwoMigration_0_0_1>();
            result[1].Should().BeOfType<TestDocumentWithTwoMigration_0_0_2>();
        }

        [Test]
        public void When_get_migrations_gt_version()
        {
            // Act
            var result = _locator.GetMigrationsGt(typeof(TestDocumentWithTwoMigration), "0.0.1").ToList();

            // Assert
            result[0].Should().BeOfType<TestDocumentWithTwoMigration_0_0_2>();
        }
    }
}