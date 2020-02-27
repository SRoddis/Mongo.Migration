using MongoDB.Driver;

namespace Mongo.Migration.Startup
{
    public class MongoMigrationSettings : IMongoMigrationSettings
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public string VersionFieldName { get; set; }

        public string RunningVersion { get; set; } = "1.0.0";

        public MongoClientSettings ClientSettings { get; set; }
    }
}