using Mongo.Migration.Exceptions;
using Mongo.Migration.Services.DiContainer;

namespace Mongo.Migration.Services.Initializers
{
    public static class MongoMigration
    {
        private static bool _isInitialized;

        private static readonly ICompoentRegistry _components;

        static MongoMigration()
        {
            _components = new ComponentRegistry();
            _components.RegisterComponents();
        }

        public static void Initialize()
        {
            if (_isInitialized) throw new AlreadyInitializedException();

            var app = _components.Get<IApplication>();
            app.Run();

            _isInitialized = true;
        }

        public static void Reset()
        {
            _isInitialized = false;
        }
    }
}