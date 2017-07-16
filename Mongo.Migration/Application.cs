using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration
{
    internal class Application : IApplication
    {
        private readonly IMongoRegistrator _mongoRegistrator;
        private readonly IMigrationLocator _migrationLocator;
        private readonly IVersionLocator _versionLocator;

        public Application(IMongoRegistrator mongoRegistrator, IMigrationLocator migrationLocator, IVersionLocator versionLocator)
        {
            _mongoRegistrator = mongoRegistrator;
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
        }

        public void Run()
        {
            _mongoRegistrator.Register();
            _migrationLocator.LoadMigrations();
            _versionLocator.LoadVersions();
        }
    }
}