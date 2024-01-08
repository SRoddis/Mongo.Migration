using FluentAssertions;

using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;

using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database;

[TestFixture]
internal class DatabaseMigrationRunner_when_migrating_down : DatabaseIntegrationTest
{
    private IDatabaseMigrationRunner _runner;

    protected override void OnSetUp(DocumentVersion databaseMigrationVersion)
    {
        base.OnSetUp(databaseMigrationVersion);

        this._runner = this._components.Get<IDatabaseMigrationRunner>();
    }

    [TearDown]
    public void TearDown()
    {
        this.Dispose();
    }

    [Test]
    public void When_database_has_migrations_Then_down_all_migrations()
    {
        this.OnSetUp(DocumentVersion.Default());

        // Arrange
        this.InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration_0_0_1(),
                new TestDatabaseMigration_0_0_2(),
                new TestDatabaseMigration_0_0_3()
            });

        // Act
        this._runner.Run(this._db);

        // Assert
        var migrations = this.GetMigrationHistory();
        migrations.Should().BeEmpty();
    }

    [Test]
    public void When_database_has_migrations_Then_down_to_selected_migration()
    {
        this.OnSetUp(new DocumentVersion("0.0.1"));

        // Arrange
        this.InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration_0_0_1(),
                new TestDatabaseMigration_0_0_2(),
                new TestDatabaseMigration_0_0_3()
            });

        // Act
        this._runner.Run(this._db);

        // Assert
        var migrations = this.GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations.Should().OnlyContain(m => m.Version == "0.0.1");
    }
}