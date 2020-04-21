using MongoDB.Driver;

namespace Mongo.Migration.Startup
{
    public class MongoMigrationSettings : IMongoMigrationSettings
    {
        public string ConnectionString { get; set; }

        public string[] Databases { get; private set; }

        public string Database
        {
            set { this.Databases = new string[] { value }; }
        }

        public string VersionFieldName { get; set; }

        public MongoClientSettings ClientSettings { get; set; }
    }
}