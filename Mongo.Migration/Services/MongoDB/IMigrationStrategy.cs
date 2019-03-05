namespace Mongo.Migration.Services.MongoDB
{
    internal interface IMigrationStrategy
    {
        void Migrate();
    }
}