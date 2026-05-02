namespace Mongo.Migration.Startup;

internal class MongoMigrationStartupSettings
{
    public bool RuntimeMigrationEnabled { get; set; }
    public bool StartupDocumentMigrationEnabled { get; set; }
    public bool DatabaseMigrationEnabled { get; set; }
}