using FluentAssertions;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [TestFixture]
    public class DatabaseMigrationWhenCreatingTest
    {
        [Test]
        public void Then_migration_has_type_DatabaseMigration()
        {
            var migration = new TestDatabaseMigration_0_0_1();

            migration.Type.Should().Be(typeof(DatabaseMigration));
        }

        [Test]
        public void Then_migration_have_version()
        {
            var migration = new TestDatabaseMigration_0_0_1();

            migration.Version.Should().Be("0.0.1");
        }

        [Test]
        public void Then_migration_should_be_created()
        {
            var migration = new TestDatabaseMigration_0_0_1();

            migration.Should().BeOfType<TestDatabaseMigration_0_0_1>();
        }
    }
}