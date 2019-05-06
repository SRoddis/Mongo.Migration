using System;
using Mongo.Migration.Exceptions;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    public static class MongoMigrationClient
    {
        private static bool _isRunning;

        public static void Initialize(IComponentRegistry componentRegistry)
        {
            if (_isRunning) throw new AlreadyInitializedException();

            var app = componentRegistry.Get<IMongoMigration>();
            app.Run();
            
            _isRunning = true;
        }
        
        public static void Initialize(IMongoClient client)
        {
            var componentRegistry = new ComponentRegistry();
            componentRegistry.RegisterComponents(client);

            Initialize(componentRegistry);
        }

        public static void Reset()
        {
            _isRunning = false;
        }
    }
}