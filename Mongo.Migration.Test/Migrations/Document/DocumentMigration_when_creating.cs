using System;
using FluentAssertions;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Document
{
    [TestFixture]
    public class DocumentMigration_when_creating
    {
        [Test]
        public void Then_migration_has_type_testClass()
        {
            // Arrange Act
            var migration = new TestDocumentWithOneMigration_0_0_1();

            // Assert
            migration.Type.Should().Be(typeof(TestDocumentWithOneMigration));
        }

        [Test]
        public void Then_migration_have_version()
        {
            // Arrange Act
            var migration = new TestDocumentWithOneMigration_0_0_1();

            // Assert
            migration.Version.Should().Be("0.0.1");
        }

        [Test]
        public void Then_migration_should_be_created()
        {
            // Arrange Act
            var migration = new TestDocumentWithOneMigration_0_0_1();

            // Assert
            migration.Should().BeOfType<TestDocumentWithOneMigration_0_0_1>();
        }
    }
}