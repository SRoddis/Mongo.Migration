using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration
{
    internal abstract class AbstractMigrationStrategy : IMigrationStrategy
    {
        private readonly ILogger<AbstractMigrationStrategy> _logger;
        private readonly DocumentVersionSerializer _serializer;
        private readonly MigrationInterceptorProvider _provider;

        protected AbstractMigrationStrategy(DocumentVersionSerializer serializer, MigrationInterceptorProvider provider)
            : this(serializer, provider, NullLoggerFactory.Instance)
        {
        }

        protected AbstractMigrationStrategy(
            DocumentVersionSerializer serializer,
            MigrationInterceptorProvider provider,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _provider = provider;
            _logger = loggerFactory.CreateLogger<AbstractMigrationStrategy>();
        }

        public void Migrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
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