using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Startup;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services
{
    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly IStartUpDocumentMigrationRunner _startUpDocumentMigrationRunner;
        private readonly IStartUpDatabaseMigrationRunner _startUpDatabaseMigrationRunner;
        private readonly IMigrationInterceptorProvider _provider;
        private readonly DocumentVersionSerializer _serializer;
        private readonly IMongoMigrationSettings _settings;

        public MigrationService(DocumentVersionSerializer serializer, IMigrationInterceptorProvider provider,
            IStartUpDocumentMigrationRunner startUpDocumentMigrationRunner, IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner, IMongoMigrationSettings settings)
            : this(serializer, provider, NullLoggerFactory.Instance, settings)
        {
            _startUpDocumentMigrationRunner = startUpDocumentMigrationRunner;
            _startUpDatabaseMigrationRunner = startUpDatabaseMigrationRunner;
        }

        private MigrationService(
            DocumentVersionSerializer serializer,
            IMigrationInterceptorProvider provider,
            ILoggerFactory loggerFactory,
            IMongoMigrationSettings settings)
        {
            _serializer = serializer;
            _provider = provider;
            _logger = loggerFactory.CreateLogger<MigrationService>();
            _settings = settings;
        }

        public void Migrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
            RegisterSerializer();

            OnStartup();
        }

        private void OnStartup()
        {
            _startUpDatabaseMigrationRunner.RunAll();
            if (!_settings.SkipDocumentMigration)
            {
                _startUpDocumentMigrationRunner.RunAll();
            }
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