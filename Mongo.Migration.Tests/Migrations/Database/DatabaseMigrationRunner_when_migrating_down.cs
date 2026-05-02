using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal class DatabaseMigrationRunnerWhenMigratingDown : DatabaseIntegrationTest
{
    [Test]
    public async Task When_database_has_migrations_Then_down_all_migrations()
    {
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
        [
            new TestDatabaseMigration001(),
            new TestDatabaseMigration002(),
            new TestDatabaseMigration003()
        ]);

        // Act
        await runner.RunAsync(Db, DocumentVersion.Default);

        // Assert
        var migrations = GetMigrationHistory();
        Assert.That(migrations, Is.Empty);
    }

    [Test]
    public async Task When_database_has_migrations_Then_down_to_selected_migration()
    {
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
        [
            new TestDatabaseMigration001(),
            new TestDatabaseMigration002(),
            new TestDatabaseMigration003()
        ]);

        // Act
        await runner.RunAsync(Db, new DocumentVersion(0, 0, 1));

        // Assert
        List<MigrationHistory> migrations = GetMigrationHistory();
        Assert.That(migrations, Is.Not.Empty);
        Assert.That(migrations, Has.One.Matches<MigrationHistory>(m => m.Version == new DocumentVersion(0, 0, 1)));
    }
}