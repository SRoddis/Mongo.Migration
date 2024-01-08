using Mongo.Migration.Documents;

using MongoDB.Driver;

namespace Mongo.Migration.Startup;

public interface IMongoMigrationSettings
{
    string ConnectionString { get; set; }

    string Database { get; set; }

    DocumentVersion DatabaseMigrationVersion { get; set; }

    string VersionFieldName { get; set; }

    MongoClientSettings ClientSettings { get; set; }
}