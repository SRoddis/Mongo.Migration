using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration.OnDeserialization
{
    internal class MigrationOnDeserialization : AbstractMigrationStrategy
    {
        private readonly MigrationInterceptorProvider _provider;

        public MigrationOnDeserialization(DocumentVersionSerializer serializer, MigrationInterceptorProvider provider) :
            base(serializer)
        {
            _provider = provider;
        }

        protected override void OnMigrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
        }
    }
}