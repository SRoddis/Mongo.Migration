using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorFactory : IMigrationInterceptorFactory
    {
        private readonly IMigrationRunner _migrationRunner;

        public MigrationInterceptorFactory(IMigrationRunner migrationRunner)
        {
            _migrationRunner = migrationRunner;
        }
        
        public IBsonSerializer Create(Type type)
        {
            var genericType = typeof(MigrationInterceptor<>).MakeGenericType(type);
            var interceptor = Activator.CreateInstance(genericType, _migrationRunner);
            return interceptor as IBsonSerializer;
        }
    }
}