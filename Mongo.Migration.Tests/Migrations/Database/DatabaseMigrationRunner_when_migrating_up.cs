using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal class DatabaseMigrationRunnerWhenMigratingUp : DatabaseIntegrationTest
{
    [Test]
    public async Task When_database_has_no_migrations_Then_all_migrations_are_used()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Act
        await runner.RunAsync(Db, DocumentVersion.Empty);

        // Assert
        var migrations = GetMigrationHistory();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(migrations, Is.Not.Empty);
            Assert.That(migrations[0].Version.ToString(), Is.EqualTo("0.0.1"));
            Assert.That(migrations[1].Version.ToString(), Is.EqualTo("0.0.2"));
            Assert.That(migrations[2].Version.ToString(), Is.EqualTo("0.0.3"));
        }
    }

    [Test]
    public async Task When_database_has_migrations_Then_latest_migrations_are_used()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();
        InsertMigrations([new TestDatabaseMigration001(), new TestDatabaseMigration002()]);

        // Act
        await runner.RunAsync(Db, DocumentVersion.Empty);

        // Assert
        var migrations = GetMigrationHistory();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(migrations, Is.Not.Empty);
            Assert.That(migrations[2].Version.ToString(), Is.EqualTo("0.0.3"));
        }
    }

    [Test]
    public async Task When_database_has_latest_version_Then_nothing_happens()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();
        InsertMigrations(
        [
            new TestDatabaseMigration001(),
            new TestDatabaseMigration002(),
            new TestDatabaseMigration003()
        ]);

        // Act
        await runner.RunAsync(Db, DocumentVersion.Empty, CancellationToken.None);

        // Assert
        var migrations = GetMigrationHistory();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(migrations, Is.Not.Empty);
            Assert.That(migrations[0].Version.ToString(), Is.EqualTo("0.0.1"));
            Assert.That(migrations[1].Version.ToString(), Is.EqualTo("0.0.2"));
            Assert.That(migrations[2].Version.ToString(), Is.EqualTo("0.0.3"));
        }
    }
}