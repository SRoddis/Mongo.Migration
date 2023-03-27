using System.Linq;
using FluentAssertions;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Locators
{
    [TestFixture]
    public class TypeMigrationLocatorWhenLocateTest
    {
        private TypeMigrationLocator _locator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _locator = new TypeMigrationLocator(new MongoMigrationAssemblyService(new MongoMigrationSettings()));
        }

        [Test]
        public void When_document_has_one_migration_Then_migrations_count_should_be_one()
        {
            var result = _locator.GetMigrations(typeof(TestDocumentWithOneMigration));

            result.Count().Should().Be(1);
        }

        [Test]
        public void When_document_has_two_migration_Then_migrations_count_should_be_two()
        {
            var result = _locator.GetMigrations(typeof(TestDocumentWithTwoMigration));

            result.Count().Should().Be(2);
        }

        [Test]
        public void When_get_latest_version_of_migrations()
        {
            var version = _locator.GetLatestVersion(typeof(TestDocumentWithTwoMigration));

            version.Should().Be("0.0.2");
        }

        [Test]
        public void When_get_migrations_gt_and_equal_version()
        {
            var result = _locator.GetMigrationsGtEq(typeof(TestDocumentWithTwoMigration), "0.0.1").ToList();

            result[0].Should().BeOfType<TestDocumentWithTwoMigration_0_0_1>();
            result[1].Should().BeOfType<TestDocumentWithTwoMigration_0_0_2>();
        }

        [Test]
        public void When_get_migrations_gt_version()
        {
            var result = _locator.GetMigrationsGt(typeof(TestDocumentWithTwoMigration), "0.0.1").ToList();

            result[0].Should().BeOfType<TestDocumentWithTwoMigration_0_0_2>();
        }
    }
}