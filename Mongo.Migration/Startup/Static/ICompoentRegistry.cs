using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    internal interface ICompoentRegistry
    {
        void RegisterMigrationOnStartup(IMongoClient client);
        
        void RegisterMigrationOnDeserialization();

        TComponent Get<TComponent>() where TComponent : class;
    }
}