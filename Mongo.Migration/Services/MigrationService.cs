using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Bson;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Startup;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Mongo.Migration.Services;

internal class MigrationService : IMigrationService
{
    private readonly ILogger<MigrationService> _logger;
    private readonly IMongoClient _client;
    private readonly MongoMigrationStartupSettings _startupSettings;
    private readonly IServiceProvider _provider;

    public MigrationService(
        ILogger<MigrationService> logger,
        IMongoClient client,
        MongoMigrationStartupSettings startupSettings,
        IServiceProvider provider)
    {
        _logger = logger;
        _client = client;
        _startupSettings = startupSettings;
        _provider = provider;
    }

    /// <inheritdoc/>
    public void RegisterBsonStatics()
    {
        BsonSerializer.RegisterSerializer(new DocumentVersionSerializer());

        if (_startupSettings.RuntimeMigrationEnabled)
        {
            MigrationBsonSerializerProvider migrationSerializerProvider = _provider.GetRequiredService<MigrationBsonSerializerProvider>();
            BsonSerializer.RegisterSerializationProvider(migrationSerializerProvider);

            var documentMigrationLocator = _provider.GetRequiredService<IMigrationLocator<IDocumentMigration>>();
            documentMigrationLocator.Initialize();
        }
    }

    /// <inheritdoc/>
    public async Task ExecuteMigrationsAsync(string databaseName, string? targetDatabaseVersion, CancellationToken cancellationToken)
    {
        if (_startupSettings.DatabaseMigrationEnabled)
        {
            await ExecuteDatabaseMigrationAsync(databaseName, targetDatabaseVersion, cancellationToken);
        }

        if (_startupSettings.StartupDocumentMigrationEnabled)
        {
            await ExecuteDocumentMigrationAsync(databaseName, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task ExecuteDatabaseMigrationAsync(string databaseName, string? targetVersion, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        DocumentVersion? typedTargetVersion = null;

        if (targetVersion is not null)
        {
            typedTargetVersion = DocumentVersion.Parse(targetVersion.AsSpan());
        }

        _logger.LogInformation("Executing database migration...");
        await using var scope = _provider.CreateAsyncScope();

        IMongoDatabase database = _client.GetDatabase(databaseName);
        IDatabaseMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
        await runner.RunAsync(database, typedTargetVersion, cancellationToken);

        _logger.LogInformation("Database migration done in {ElapsedMs} ms", sw.ElapsedMilliseconds);
    }

    /// <inheritdoc/>
    public async Task ExecuteDocumentMigrationAsync(string databaseName, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();
        _logger.LogInformation("Executing document migration...");
        await using var scope = _provider.CreateAsyncScope();

        IMongoDatabase database = _client.GetDatabase(databaseName);
        IStartUpDocumentMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IStartUpDocumentMigrationRunner>();
        await runner.RunAllAsync(database, cancellationToken);

        _logger.LogInformation("Database migration done in {ElapsedMs} ms", sw.ElapsedMilliseconds);
    }
}