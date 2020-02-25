namespace Mongo.Migration.Startup
{
    public interface IMongoMigrationSettings
    {
        string ConnectionString { get; set; }
        string Database { get; set; }
        string VersionFieldName { get; set; }
    }
}