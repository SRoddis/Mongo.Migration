using System;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptorProvider : IBsonSerializationProvider
    {
        private readonly IMigrationInterceptorFactory _migrationInterceptorFactory;

        public MigrationInterceptorProvider(IMigrationInterceptorFactory migrationInterceptorFactory)
        {
            _migrationInterceptorFactory = migrationInterceptorFactory;
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            if (ShouldBeMigrated(type))
                return _migrationInterceptorFactory.Create(type);

            return null;
        }

        private static bool ShouldBeMigrated(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IDocument)) && type != typeof(BsonDocument);
        }
    }
}