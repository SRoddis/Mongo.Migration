namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationSettings
    {
        public string ConnectionString { get; set; }
        
        public string Database { get; set; }

        public string RunningVersion { get; set; }
    }
}