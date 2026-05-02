using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

public interface IDatabaseMigration : IMigration
{
    Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken);

    Task DownAsync(IMongoDatabase db, CancellationToken cancellationToken);
}