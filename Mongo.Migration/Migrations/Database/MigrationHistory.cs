using Mongo.Migration.Documents;

using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Database;

public class MigrationHistory
{
    public ObjectId Id { get; set; }

    public string MigrationId { get; set; }

    public DocumentVersion Version { get; set; }
}