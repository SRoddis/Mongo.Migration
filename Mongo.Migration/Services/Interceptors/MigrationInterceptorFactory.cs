using System;
using Mongo.Migration.Migrations;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorFactory : IMigrationInterceptorFactory
    {
        private readonly IMigrationRunner _migrationRunner;
        private readonly IVersionService _versionService;

        public MigrationInterceptorFactory(IMigrationRunner migrationRunner, IVersionService versionService)
        {
            _migrationRunner = migrationRunner;
            _versionService = versionService;
        }
        
        public IBsonSerializer Create(Type type)
        {
            var genericType = typeof(MigrationInterceptor<>).MakeGenericType(type);
            var interceptor = Activator.CreateInstance(genericType, _migrationRunner, _versionService);
            return interceptor as IBsonSerializer;
        }
    }
}