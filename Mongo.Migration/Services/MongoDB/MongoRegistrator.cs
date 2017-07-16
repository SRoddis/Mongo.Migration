using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services.Interceptors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.MongoDB
{
    internal class MongoRegistrator : IMongoRegistrator
    {
        private readonly MigrationInterceptorProvider _provider;

        private readonly DocumentVersionSerializer _serializer;

        public MongoRegistrator(DocumentVersionSerializer serializer, MigrationInterceptorProvider provider)
        {
            _serializer = serializer;
            _provider = provider;
        }

        public void Register()
        {
            RegisterSerializationProvider();
            RegisterSerializer();
        }

        private void RegisterSerializationProvider()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
        }

        private void RegisterSerializer()
        {
            var foundSerializer = BsonSerializer.LookupSerializer(_serializer.ValueType);
            
            if (foundSerializer == null)
                BsonSerializer.RegisterSerializer(_serializer.ValueType, _serializer);
        }
    }
}