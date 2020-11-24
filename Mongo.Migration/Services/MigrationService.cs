using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Services.Interceptors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly ICollectionMigrationRunner _migrationRunner;
        private readonly IMigrationInterceptorProvider _provider;
        private readonly DocumentVersionSerializer _serializer;

        public MigrationService(DocumentVersionSerializer serializer, IMigrationInterceptorProvider provider,
            ICollectionMigrationRunner migrationRunner)
            : this(serializer, provider, NullLoggerFactory.Instance)
        {
            _migrationRunner = migrationRunner;
        }

        private MigrationService(
            DocumentVersionSerializer serializer,
            IMigrationInterceptorProvider provider,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _provider = provider;
            _logger = loggerFactory.CreateLogger<MigrationService>();
        }

        public void Migrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
            RegisterSerializer();

            OnStartup();
        }

        private void OnStartup()
        {
            _migrationRunner.RunAll();
        }

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