namespace Mongo.Migration.Migrations.Database;

internal interface IStartUpDatabaseMigrationRunner
{
    void RunAll();
}