using Mongo.Migration.Documents.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration
{
    internal abstract class AbstractMigrationStrategy : IMigrationStrategy
    {
        private readonly DocumentVersionSerializer _serializer;

        protected AbstractMigrationStrategy(DocumentVersionSerializer serializer)
        {
            _serializer = serializer;
        }

        public void Migrate()
        {
            RegisterSerializer();
            OnMigrate();
        }

        protected abstract void OnMigrate();

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