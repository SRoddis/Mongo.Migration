using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration
{
    internal abstract class AbstractMigrationStrategy : IMigrationStrategy
    {
        private readonly DocumentVersionSerializer _serializer;

        private readonly ILogger<AbstractMigrationStrategy> _logger;

        protected AbstractMigrationStrategy(DocumentVersionSerializer serializer)
            : this(serializer, NullLoggerFactory.Instance)
        {
        }

        protected AbstractMigrationStrategy(DocumentVersionSerializer serializer, ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _logger = loggerFactory.CreateLogger<AbstractMigrationStrategy>();
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
            catch (BsonSerializationException ex)
            {
                // Catch if Serializer was registered already ... not great, I know.
                // We have to do this, because there is always a default DocumentVersionSerialzer.
                // BsonSerializer.LookupSerializer(), does not work.
                
                _logger.LogError(ex, ex.GetType().ToString());
            }
        }
    }
}