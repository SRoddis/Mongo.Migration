namespace Mongo.Migration.Migrations
{
    internal interface IDatabaseMigrationRunner
    {
        void RunAll();
    }
}