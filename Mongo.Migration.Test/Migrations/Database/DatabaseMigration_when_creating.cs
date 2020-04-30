using FluentAssertions;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [TestFixture]
    public class DatabaseMigration_when_creating
    {
        [Test]
        public void Then_migration_has_type_DatabaseMigration()
        {
            // Arrange Act
            var migration = new TestDatabaseMigration_0_0_1();

            // Assert
            migration.Type.Should().Be(typeof(DatabaseMigration));
        }

        [Test]
        public void Then_migration_have_version()
        {
            // Arrange Act
            var migration = new TestDatabaseMigration_0_0_1();

            // Assert
            migration.Version.Should().Be("0.0.1");
        }

        [Test]
        public void Then_migration_should_be_created()
        {
            // Arrange Act
            var migration = new TestDatabaseMigration_0_0_1();

            // Assert
            migration.Should().BeOfType<TestDatabaseMigration_0_0_1>();
        }
    }
}