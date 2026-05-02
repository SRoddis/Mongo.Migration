using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Document;

internal interface IStartUpDocumentMigrationRunner
{
    Task RunAllAsync(IMongoDatabase database, CancellationToken cancellationToken);
}