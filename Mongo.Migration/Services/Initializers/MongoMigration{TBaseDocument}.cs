using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services.DiContainer;

namespace Mongo.Migration.Services.Initializers
{
    public static class MongoMigration<TBaseDocument>
    {
        private static bool _isInitialized;

        private static readonly IComponentRegistry _components;

        static MongoMigration()
        {
            _components = new ComponentRegistry();
        }

        public static void RegisterComponents(Func<TBaseDocument, DocumentVersion> versionGetter,
            Action<TBaseDocument, DocumentVersion> versionSetter)
        {
            _components.RegisterComponents(versionGetter, versionSetter);
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