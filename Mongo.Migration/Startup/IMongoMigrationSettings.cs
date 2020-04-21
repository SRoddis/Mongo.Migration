using MongoDB.Driver;

namespace Mongo.Migration.Startup
{
    public interface IMongoMigrationSettings
    {
        string ConnectionString { get; }
        string[] Databases { get; }
        string VersionFieldName { get; }
        MongoClientSettings ClientSettings { get; }
    }
}