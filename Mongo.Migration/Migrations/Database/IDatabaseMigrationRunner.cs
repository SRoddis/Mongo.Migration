using Mongo.Migration.Documents;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

public interface IDatabaseMigrationRunner
{
    Task RunAsync(IMongoDatabase db, DocumentVersion? targetVersion = null, CancellationToken cancellationToken = default);
}