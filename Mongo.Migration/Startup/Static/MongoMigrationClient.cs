using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations.Adapters;

using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static;

public static class MongoMigrationClient
{
    private static bool _isRunning;

    public static void Initialize(IComponentRegistry componentRegistry)
    {
        if (_isRunning)
        {
            throw new AlreadyInitializedException();
        }

        var app = componentRegistry.Get<IMongoMigration>();
        app.Run();

        _isRunning = true;
    }

    public static void Initialize(IMongoClient client, IContainerAdapter containerAdapter)
    {
        var componentRegistry = new ComponentRegistry(new MongoMigrationSettings(), containerAdapter);
        componentRegistry.RegisterComponents(client);

        Initialize(componentRegistry);
    }

    public static void Initialize(IMongoClient client, IMongoMigrationSettings settings = null, IContainerAdapter containerAdapter = null)
    {
        var componentRegistry = new ComponentRegistry(settings ?? new MongoMigrationSettings(), containerAdapter);
        componentRegistry.RegisterComponents(client);

        Initialize(componentRegistry);
    }

    public static void Reset()
    {
        _isRunning = false;
    }
}