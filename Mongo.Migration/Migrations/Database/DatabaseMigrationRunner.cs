using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
{
    private readonly IDatabaseVersionService _databaseVersionService;

    private readonly ILogger _logger;

    private readonly Type _databaseMigrationType = typeof(DatabaseMigration);

    private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;

    public DatabaseMigrationRunner(
        IDatabaseTypeMigrationDependencyLocator migrationLocator,
        IDatabaseVersionService databaseVersionService,
        ILogger<DatabaseMigrationRunner> logger)
    {
        _migrationLocator = migrationLocator;
        _databaseVersionService = databaseVersionService;
        _logger = logger;
    }

    public async Task RunAsync(IMongoDatabase db, DocumentVersion? targetVersion = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Database migration started.");
        DocumentVersion databaseVersion = await _databaseVersionService.GetLatestDatabaseVersionAsync(db, cancellationToken);
        DocumentVersion currentOrLatest = targetVersion is not null && targetVersion > DocumentVersion.Empty
            ? targetVersion.Value
            : _databaseVersionService.GetLatestMigrationVersion();

        if (databaseVersion != currentOrLatest)
        {
            await MigrateUpOrDownAsync(db, databaseVersion, currentOrLatest, cancellationToken);
        }
        
        _logger.LogInformation("Database migration finished.");
    }

    private async Task MigrateUpOrDownAsync(
        IMongoDatabase db,
        DocumentVersion databaseVersion,
        DocumentVersion to,
        CancellationToken cancellationToken)
    {
        if (databaseVersion > to)
        {
            await MigrateDownAsync(db, databaseVersion, to, cancellationToken);
            return;
        }

        await MigrateUpAsync(db, databaseVersion, to, cancellationToken);
    }

    private async Task MigrateUpAsync(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion, CancellationToken cancellationToken)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromTo(_databaseMigrationType, currentVersion, toVersion);

        foreach (var migration in migrations)
        {
            _logger.LogInformation("Database Migration Up: {Type}:{Version} ", currentVersion.GetType(), migration.Version);

            await migration.UpAsync(db, cancellationToken);
            await _databaseVersionService.SaveAsync(db, migration, cancellationToken);

            _logger.LogInformation("Database Migration Up finished successful: {Type}:{Version} ", migration.GetType(), migration.Version);
        }
    }

    private async Task MigrateDownAsync(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion, CancellationToken cancellationToken)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromToDown(_databaseMigrationType, currentVersion, toVersion);

        foreach (var migration in migrations)
        {
            _logger.LogInformation("Database Migration Down: {Type}:{Version} ", migration.GetType(), migration.Version);

            await migration.DownAsync(db, cancellationToken);
            await _databaseVersionService.RemoveAsync(db, migration, cancellationToken);

            _logger.LogInformation("Database Migration Down finished successful: {Type}:{Version} ", migration.GetType(), migration.Version);
        }
    }
}