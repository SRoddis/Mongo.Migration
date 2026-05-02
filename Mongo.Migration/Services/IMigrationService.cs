namespace Mongo.Migration.Services;

public interface IMigrationService
{
    /// <summary>
    /// Register all serializer and other settings to MongoDB.Driver.
    /// This enables the DocumentRuntime migration if it has been setup
    /// </summary>
    /// <remarks>Must be called once, before any mongo interaction</remarks>
    void RegisterBsonStatics();

    /// <summary>
    /// Immediately execute all migration setup
    /// Don't do anything if only document runtime migration added
    /// </summary>
    /// <param name="databaseName">The target database</param>
    /// <param name="targetDatabaseVersion">The expected database version (latest of null)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteMigrationsAsync(string databaseName, string? targetDatabaseVersion, CancellationToken cancellationToken);

    /// <summary>
    /// Immediately execute the database migrations
    /// </summary>
    /// <param name="databaseName">The target database</param>
    /// <param name="targetVersion">The expected database version (latest of null)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteDatabaseMigrationAsync(string databaseName, string? targetVersion, CancellationToken cancellationToken);

    /// <summary>
    /// Immediately execute the document migrations
    /// </summary>
    /// <param name="databaseName">The target database</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteDocumentMigrationAsync(string databaseName, CancellationToken cancellationToken);
}