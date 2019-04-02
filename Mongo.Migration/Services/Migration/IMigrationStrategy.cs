namespace Mongo.Migration.Services.Migration
{
    internal interface IMigrationStrategy
    {
        void Migrate();
    }
}