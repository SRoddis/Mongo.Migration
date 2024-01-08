using Mongo.Migration.Documents;

using MongoDB.Driver;

namespace Mongo.Migration.Startup;

public class MongoMigrationSettings : IMongoMigrationSettings
{
    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public DocumentVersion DatabaseMigrationVersion { get; set; } = DocumentVersion.Empty();

    public string VersionFieldName { get; set; }

    public MongoClientSettings ClientSettings { get; set; }
}