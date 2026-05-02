using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
public class DatabaseMigrationWhenCreating
{
    [Test]
    public void Then_migration_has_type_DatabaseMigration()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        Assert.That(migration.Type, Is.EqualTo(typeof(DatabaseMigration)));
    }

    [Test]
    public void Then_migration_have_version()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        Assert.That(migration.Version.ToString(), Is.EqualTo("0.0.1"));
    }

    [Test]
    public void Then_migration_should_be_created()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        Assert.That(migration, Is.TypeOf<TestDatabaseMigration001>());
    }
}