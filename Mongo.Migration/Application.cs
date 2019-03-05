using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration
{
    internal class Application : IApplication
    {
        private readonly IMigrationLocator _migrationLocator;
        private readonly IVersionLocator _versionLocator;
        private readonly IMigrationStrategy _migrationStrategy;

        public Application(IMigrationLocator migrationLocator, IVersionLocator versionLocator,
            IMigrationStrategy migrationStrategy)
        {
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
            _migrationStrategy = migrationStrategy;
        }

        public void Run()
        {
            _migrationLocator.LoadMigrations();
            _versionLocator.LoadVersions();
            
            _migrationStrategy.Migrate();
        }
    }
}