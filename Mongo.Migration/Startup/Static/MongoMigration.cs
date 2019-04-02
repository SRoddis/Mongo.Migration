using Mongo.Migration.Exceptions;

namespace Mongo.Migration.Services.Startup.Static
{
    public static class MongoMigration
    {
        private static bool _isInitialized;

        private static readonly ICompoentRegistry _components;

        static MongoMigration()
        {
            _components = new ComponentRegistry();
        }
        
        public static void MigrationOnStartup()
        {
            _components.RegisterMigrationOnStartup();

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