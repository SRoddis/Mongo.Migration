﻿using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services.DiContainer;

namespace Mongo.Migration.Services.Initializers
{
    public static class MongoMigration
    {
        private static bool _isInitialized;

        private static readonly IComponentRegistry _components;

        static MongoMigration()
        {
            _components = new ComponentRegistry();
            _components.RegisterComponents<IDocument>(d => d.Version, (d, v) => d.Version = v);
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