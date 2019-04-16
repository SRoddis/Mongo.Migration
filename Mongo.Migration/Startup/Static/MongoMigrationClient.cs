using Mongo.Migration.Exceptions;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    public static class MongoMigrationClient
    {
        private static bool _isRunning;

        private static readonly ICompoentRegistry _components;

        static MongoMigrationClient()
        {
            _components = new ComponentRegistry();
        }
        
        public static void Migrate(IMongoClient client)
        {
            _components.RegisterMigrationOnStartup(client);

            Run();
        }

        public static void MigrationOnDeserialization()
        {
            _components.RegisterMigrationOnDeserialization();

            Run();
        }

        private static void Run()
        {
            if (_isRunning) throw new AlreadyInitializedException();

            var app = _components.Get<IMongoMigration>();
            app.Run();

            _isRunning = true;
        }

        public static void Reset()
        {
            _isRunning = false;
        }
    }
}