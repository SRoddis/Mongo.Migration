using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration
{
    internal class Application : IApplication
    {
        private readonly IMongoRegistrater _registrater;
        private readonly IMigrationLocator _migrationLocator;
        private readonly IVersionLocator _versionLocator;

        public Application(IMongoRegistrater registrater, IMigrationLocator migrationLocator, IVersionLocator versionLocator)
        {
            _registrater = registrater;
            _migrationLocator = migrationLocator;
            _versionLocator = versionLocator;
        }

        public void Run()
        {
            _registrater.Registrate();
            _migrationLocator.LoadMigrations();
            _versionLocator.LoadVersions();
        }
    }
}