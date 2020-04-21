using MongoDB.Driver;

namespace Mongo.Migration.Startup
{
    public class MongoMultiDbMigrationSettings : IMongoMigrationSettings
    {
        public string ConnectionString { get; set; }

        public string[] Databases { get; set; }

        public string VersionFieldName { get; set; }

        public MongoClientSettings ClientSettings { get; set; }
    }
}