using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;

namespace Mongo.Migration.Services.Migration.OnStartup
{
    internal class MigrationOnStartup : AbstractMigrationStrategy
    {
        private readonly ICollectionMigrationRunner _migrationRunner;

        public MigrationOnStartup(
            DocumentVersionSerializer serializer,
            MigrationInterceptorProvider provider,
            ICollectionMigrationRunner migrationRunner) :
            base(serializer, provider)
        {
            _migrationRunner = migrationRunner;
        }

        protected override void OnMigrate()
        {
            _migrationRunner.RunAll();
        }
    }
}