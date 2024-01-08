using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static;

public interface IComponentRegistry
{
    void RegisterComponents(IMongoClient client);

    TComponent Get<TComponent>()
        where TComponent : class;
}