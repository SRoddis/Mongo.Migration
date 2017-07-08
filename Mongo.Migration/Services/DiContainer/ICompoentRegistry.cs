namespace Mongo.Migration.Services.DiContainer
{
    internal interface ICompoentRegistry
    {
        void RegisterComponents();

        TComponent Get<TComponent>() where TComponent : class;
    }
}