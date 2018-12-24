using System;
using Mongo.Migration.Migrations;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorFactory<TBaseDocument> : IMigrationInterceptorFactory
    {
        private readonly IMigrationRunner<TBaseDocument> _migrationRunner;

        public MigrationInterceptorFactory(IMigrationRunner<TBaseDocument> migrationRunner)
        {
            _migrationRunner = migrationRunner;
        }
        
        public IBsonSerializer Create(Type type)
        {
            var genericType = typeof(MigrationInterceptor<,>).MakeGenericType(type, typeof(TBaseDocument));
            var interceptor = Activator.CreateInstance(genericType, _migrationRunner);
            return interceptor as IBsonSerializer;
        }
    }
}