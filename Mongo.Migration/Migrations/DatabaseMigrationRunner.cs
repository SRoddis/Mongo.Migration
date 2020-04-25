using Mongo.Migration.Startup;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly IAdvancedMigrationRunner _migrationRunner;

        private readonly string _databaseName;

        private readonly string _runningVersion;

        public DatabaseMigrationRunner(
            IMongoMigrationSettings settings,
            IAdvancedMigrationRunner migrationRunner)
            : this(
                new MongoClient(settings.ConnectionString),
                migrationRunner,
                settings.RunningVersion)
        {
            _databaseName = settings.Database;
        }

        public DatabaseMigrationRunner(
            IMongoClient client,
            IAdvancedMigrationRunner migrationRunner,
            string runningVersion)
        {
            _runningVersion = runningVersion;
            _client = client;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            _migrationRunner.Run(_client.GetDatabase(_databaseName), _runningVersion);
        }
    }
}