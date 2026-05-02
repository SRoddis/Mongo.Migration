using Mongo.Migration.Documents;

using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Database;

public record MigrationHistory
{
    public ObjectId Id { get; init; }

    public required string MigrationId { get; init; }

    public DocumentVersion Version { get; init; }
}