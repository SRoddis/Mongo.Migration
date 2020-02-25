namespace Mongo.Migration.Startup
{
    public class MongoMigrationSettings : IMongoMigrationSettings
    {
        public string ConnectionString { get; set; }
        
        public string Database { get; set; }

        public string VersionFieldName { get; set; }
    }
}