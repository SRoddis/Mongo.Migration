using Mongo.Migration.Exceptions;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    public static class MongoMigration
    {
        private static bool _isInitialized;

        private static readonly ICompoentRegistry _components;

        static MongoMigration()
        {
            _components = new ComponentRegistry();
        }
        
        public static void MigrationOnStartup(IMongoClient client)
        {
            _components.RegisterMigrationOnStartup(client);

            Initialize();
        }

        public static void MigrationOnDeserialization()
        {
            _components.RegisterMigrationOnDeserialization();

            Initialize();
        }

        private static void Initialize()
        {
            if (_isInitialized) throw new AlreadyInitializedException();

            var app = _components.Get<IMongoMigration>();
            app.Run();

            _isInitialized = true;
        }

        public static void Reset()
        {
            _isInitialized = false;
        }
    }
}