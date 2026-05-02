using Mongo.Migration.Documents;

using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

public abstract class DatabaseMigration : IDatabaseMigration
{
    protected DatabaseMigration(string version)
    {
        Version = DocumentVersion.Parse(version.AsSpan());
    }

    public DocumentVersion Version { get; }

    public Type Type => typeof(DatabaseMigration);

    public abstract Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken);

    public abstract Task DownAsync(IMongoDatabase db, CancellationToken cancellationToken);
}