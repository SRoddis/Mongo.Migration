using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Driver;

namespace Mongo.Migration.Services;

internal class DatabaseVersionService : IDatabaseVersionService
{
    private const string MigrationsCollectionName = "_migrations";

    private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;

    public DatabaseVersionService(IDatabaseTypeMigrationDependencyLocator migrationLocator)
    {
        _migrationLocator = migrationLocator;
    }

    public DocumentVersion GetLatestMigrationVersion()
    {
        return _migrationLocator.GetLatestVersion(typeof(DatabaseMigration));
    }

    public async Task<DocumentVersion> GetLatestDatabaseVersionAsync(IMongoDatabase db, CancellationToken cancellationToken)
    {
        var cursor = await GetMigrationsCollection(db)
            .FindAsync(m => true, cancellationToken: cancellationToken);
        var migrations = await cursor.ToListAsync(cancellationToken);

        if (migrations is { Count: 0 })
        {
            return DocumentVersion.Default;
        }

        return migrations.Max(m => m.Version);
    }

    public async Task SaveAsync(IMongoDatabase db, IDatabaseMigration migration, CancellationToken cancellationToken)
    {
        await GetMigrationsCollection(db)
            .InsertOneAsync(
                new()
                {
                    MigrationId = migration.GetType().ToString(),
                    Version = migration.Version
                },
                cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync(IMongoDatabase db, IDatabaseMigration migration, CancellationToken cancellationToken)
    {
        await GetMigrationsCollection(db)
            .DeleteOneAsync(
                Builders<MigrationHistory>.Filter.Eq(mh => mh.MigrationId, migration.GetType().ToString()),
                cancellationToken: cancellationToken);
    }

    private static IMongoCollection<MigrationHistory> GetMigrationsCollection(IMongoDatabase db)
    {
        return db.GetCollection<MigrationHistory>(MigrationsCollectionName);
    }
}