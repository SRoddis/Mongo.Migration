using Mongo.Migration.Startup;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal class StartUpDatabaseMigrationRunner : IStartUpDatabaseMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly IDatabaseMigrationRunner _migrationRunner;

        private readonly string _databaseName;

        private readonly string _runningVersion;

        public StartUpDatabaseMigrationRunner(
            IMongoMigrationSettings settings,
            IDatabaseMigrationRunner migrationRunner)
            : this(
                new MongoClient(settings.ConnectionString),
                migrationRunner,
                settings.RunningVersion)
        {
            _databaseName = settings.Database;
        }

        public StartUpDatabaseMigrationRunner(
            IMongoClient client,
            IDatabaseMigrationRunner migrationRunner,
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