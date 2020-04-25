using System;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorFactory : IMigrationInterceptorFactory
    {
        private readonly IDocumentMigrationRunner _migrationRunner;
        private readonly IVersionService _versionService;

        public MigrationInterceptorFactory(IDocumentMigrationRunner migrationRunner, IVersionService versionService)
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