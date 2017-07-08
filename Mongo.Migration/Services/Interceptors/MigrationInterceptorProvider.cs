using System;
using System.Linq;
using Mongo.Migration.Migrations;
using Mongo.Migration.Models;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorProvider : IBsonSerializationProvider
    {
        private readonly IMigrationRunner _runner;

        public MigrationInterceptorProvider(IMigrationRunner runner)
        {
            _runner = runner;
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            if (IsNoMigrateDocument(type))
                return null;

            return CreateMigrationInterceptorInstance(type);
        }

        private static bool IsNoMigrateDocument(Type type)
        {
            return !type.GetInterfaces().Contains(typeof(IDocument));
        }

        private IBsonSerializer CreateMigrationInterceptorInstance(Type type)
        {
            var genericType = typeof(MigrationInterceptor<>).MakeGenericType(type);
            var interceptor = Activator.CreateInstance(genericType, _runner);
            return interceptor as IBsonSerializer;
        }
    }
}