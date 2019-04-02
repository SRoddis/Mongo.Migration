namespace Mongo.Migration.Startup.Static
{
    internal interface ICompoentRegistry
    {
        void RegisterMigrationOnStartup();
        
        void RegisterMigrationOnDeserialization();

        TComponent Get<TComponent>() where TComponent : class;
    }
}