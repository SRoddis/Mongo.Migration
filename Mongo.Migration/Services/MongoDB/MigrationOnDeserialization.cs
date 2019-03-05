using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services.Interceptors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.MongoDB
{
    internal class MigrationOnDeserialization : IMigrationStrategy
    {
        private readonly MigrationInterceptorProvider _provider;

        private readonly DocumentVersionSerializer _serializer;

        public MigrationOnDeserialization(DocumentVersionSerializer serializer, MigrationInterceptorProvider provider)
        {
            _serializer = serializer;
            _provider = provider;
        }

        public void Migrate()
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
            try
            {
                BsonSerializer.RegisterSerializer(_serializer.ValueType, _serializer);
            }
            catch (BsonSerializationException exception)
            {
                // Catch if Serializer was registered alread... not great, I know.
                // We have to do this, because there is always a default DocumentVersionSerialzer.
                // BsonSerializer.LookupSerializer(), does not work.
            }
        }
    }
}