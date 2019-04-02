namespace Mongo.Migration.Services.Startup.Static
{
    internal interface ICompoentRegistry
    {
        void RegisterComponents();

        TComponent Get<TComponent>() where TComponent : class;
    }
}