using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;

using MongoDB.Driver;

namespace Mongo.Migration.Services;

public interface IDatabaseVersionService
{
    DocumentVersion GetLatestMigrationVersion();

    Task<DocumentVersion> GetLatestDatabaseVersionAsync(IMongoDatabase db, CancellationToken cancellationToken);

    Task SaveAsync(IMongoDatabase db, IDatabaseMigration migration, CancellationToken cancellationToken);

    Task RemoveAsync(IMongoDatabase db, IDatabaseMigration migration, CancellationToken cancellationToken);
}