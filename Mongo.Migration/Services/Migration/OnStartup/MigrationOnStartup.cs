using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;

namespace Mongo.Migration.Services.Migration.OnStartup
{
    internal class MigrationOnStartup : AbstractMigrationStrategy
    {
        private readonly ICollectionMigrationRunner _migrationRunner;

        public MigrationOnStartup(DocumentVersionSerializer serializer, ICollectionMigrationRunner migrationRunner) :
            base(serializer)
        {
            _migrationRunner = migrationRunner;
        }

        protected override void OnMigrate()
        {
            _migrationRunner.RunAll();
        }
    }
}