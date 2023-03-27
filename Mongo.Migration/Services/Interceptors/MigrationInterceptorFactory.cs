using System;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorFactory : IMigrationInterceptorFactory
    {
        private readonly IDocumentVersionService _documentVersionService;

        private readonly IDocumentMigrationRunner _migrationRunner;

        public MigrationInterceptorFactory(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
        {
            _migrationRunner = migrationRunner;
            _documentVersionService = documentVersionService;
        }

        public IBsonSerializer Create(Type type)
        {
            var genericType = typeof(MigrationInterceptor<>).MakeGenericType(type);
            var interceptor = Activator.CreateInstance(genericType, _migrationRunner, _documentVersionService);
            return interceptor as IBsonSerializer;
        }
    }
}